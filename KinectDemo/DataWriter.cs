using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Windows;

namespace KinectDemo
{
    public class DataWriter
    {
        private const String OUTPUT_DIRECTORY = @"C:\KinectData\";
        private const int BUFFER_SIZE = 50;

        private MainWindow mainWindow;
        private List<IReadOnlyList<CameraSpacePoint>> _buffer;
        private ConcurrentQueue<IReadOnlyList<CameraSpacePoint>> _exposedBuffer;
        private string _currentFileName;
        private StreamWriter _file;
        private Thread _loop;
        private bool _recording;

        public DataWriter(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public bool StartRecording()
        {
            if(_file != null)
            {
                MessageBox.Show("fuck off");
                return false;
            }
            _recording = true;
            _currentFileName = CreateNewRecordFile();
            _buffer = new List<IReadOnlyList<CameraSpacePoint>>();
            _exposedBuffer = new ConcurrentQueue<IReadOnlyList<CameraSpacePoint>>();
            _file = new StreamWriter(OUTPUT_DIRECTORY + _currentFileName, true, Encoding.ASCII, 1024);
            _file.AutoFlush = true;
            _loop = new Thread(Loop);
            _loop.Start();

            return true;
        }

        private string CreateNewRecordFile()
        {
            string[] existingFiles = Directory.GetFiles(OUTPUT_DIRECTORY);
            int parsed;
            int maxNumber = existingFiles.Length == 0 ? 0 : Array.ConvertAll(existingFiles, 
                str => int.TryParse(new FileInfo(str).Name, out parsed) ? parsed : 0).Max();
            string newName = (maxNumber + 1).ToString();

            File.Create(OUTPUT_DIRECTORY + newName).Dispose();
            return newName;
        }

        private void Loop()
        {
            try
            {
                while (_recording)
                {
                    MoveFromExposedBufferToInner(false);
                    if (_buffer.Count >= BUFFER_SIZE)
                    {
                        FlushBuffer();
                    }
                    Thread.Sleep(10);
                }
            }
            catch (ThreadAbortException e)
            {
                MoveFromExposedBufferToInner(true);
                FlushBuffer();
                _file.Close();
                ClearFields();
                AskRenameFile();
            }
        }

        private void ClearFields()
        {
            _buffer = null;
            _exposedBuffer = null;
            _file = null;
            _currentFileName = null;
            _loop = null;
        }

        private void AskRenameFile()
        {
            while(mainWindow.isRecording())
            {
                Thread.Sleep(100);
            }
            string newName = Microsoft.VisualBasic.Interaction.InputBox("Enter name for file " + _currentFileName, "Rename file");
            File.Move(OUTPUT_DIRECTORY + _currentFileName, OUTPUT_DIRECTORY + newName + ".csv");
        }

        public void Reset()
        {
            if (_file != null)
            {
                _file.Close();
            }
            ClearFields();
        }

        public void EndRecording()
        {
            _recording = false;
            _loop.Abort();
        }

        public void AddFaceData(IReadOnlyList<CameraSpacePoint> vertices)
        {
            if (!_recording)
            {
                return;
            }
            _exposedBuffer.Enqueue(vertices);
            mainWindow.SetExposedBufferSize(_exposedBuffer.Count, this);
        }
       
        private void MoveFromExposedBufferToInner(bool getAll)
        {
            IReadOnlyList<CameraSpacePoint> points;
            while (getAll || _buffer.Count <= BUFFER_SIZE)
            {
                if (!_exposedBuffer.TryDequeue(out points))
                {
                    break;
                }
                _buffer.Add(points);
                mainWindow.SetBufferSize(_buffer.Count, this);
                mainWindow.SetExposedBufferSize(_exposedBuffer.Count, this);
            }
        }

        private void FlushBuffer()
        {
            lock (this)
            {
                var i = 0;
                _buffer.ForEach(points =>
                {
                    _file.WriteLineAsync(ConvertPointsListToString(points)).Wait();
                    mainWindow.SetBufferSize(_buffer.Count - ++i, this);
                });
                _buffer.Clear();
            }
        }

        private string ConvertPointsListToString(IReadOnlyList<CameraSpacePoint> vertices)
        {
            string result = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString();
            for (var i = 0; i < vertices.Count; ++i) {
                result += ";" + ConvertPointToString(i, vertices[i]);
            }
            return result;
        }

        private string ConvertPointToString(int n, CameraSpacePoint point)
        {
            return string.Format("{0} {1} {2} {3}", n.ToString(), point.X, point.Y, point.Z);
        }
    }
}
