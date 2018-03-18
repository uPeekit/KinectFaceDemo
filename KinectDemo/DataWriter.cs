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
        private List<IReadOnlyList<CameraSpacePoint>> buffer;
        private ConcurrentQueue<IReadOnlyList<CameraSpacePoint>> exposedBuffer;
        private string currentFileName;
        private StreamWriter file;
        private Thread loop;
        private bool recording;

        public DataWriter(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public bool StartRecording()
        {
            if(file != null)
            {
                MessageBox.Show("fuck off");
                return false;
            }
            recording = true;
            currentFileName = CreateNewRecordFile();
            buffer = new List<IReadOnlyList<CameraSpacePoint>>();
            exposedBuffer = new ConcurrentQueue<IReadOnlyList<CameraSpacePoint>>();
            file = new StreamWriter(OUTPUT_DIRECTORY + currentFileName, true, Encoding.ASCII, 1024);
            file.AutoFlush = true;
            loop = new Thread(Loop);
            loop.Start();

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
            mainWindow.SetFileName(currentFileName + ".csv");

            return newName;
        }

        private void Loop()
        {
            try
            {
                while (recording)
                {
                    MoveFromExposedBufferToInner(false);
                    if (buffer.Count >= BUFFER_SIZE)
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
                file.Close();
                ClearFields();
                AskRenameFile();
            }
        }

        private void ClearFields()
        {
            buffer = null;
            exposedBuffer = null;
            file = null;
            currentFileName = null;
            loop = null;
        }

        private void AskRenameFile()
        {
            while(mainWindow.IsRecording())
            {
                Thread.Sleep(100);
            }
            string newName = Microsoft.VisualBasic.Interaction.InputBox("Enter name for file " + currentFileName, "Rename file");
            File.Move(OUTPUT_DIRECTORY + currentFileName, OUTPUT_DIRECTORY + newName + ".csv");
        }

        public void Reset()
        {
            if (file != null)
            {
                file.Close();
            }
            ClearFields();
        }

        public void EndRecording()
        {
            recording = false;
            loop.Abort();
        }

        public void AddFaceData(IReadOnlyList<CameraSpacePoint> vertices)
        {
            if (!recording)
            {
                return;
            }
            exposedBuffer.Enqueue(vertices);
            mainWindow.SetExposedBufferSize(exposedBuffer.Count, this);
        }
       
        private void MoveFromExposedBufferToInner(bool getAll)
        {
            IReadOnlyList<CameraSpacePoint> points;
            while (getAll || buffer.Count <= BUFFER_SIZE)
            {
                if (!exposedBuffer.TryDequeue(out points))
                {
                    break;
                }
                buffer.Add(points);
                mainWindow.SetBufferSize(buffer.Count, this);
                mainWindow.SetExposedBufferSize(exposedBuffer.Count, this);
            }
        }

        private void FlushBuffer()
        {
            lock (this)
            {
                var i = 0;
                buffer.ForEach(points =>
                {
                    file.WriteLineAsync(ConvertPointsListToString(points)).Wait();
                    mainWindow.SetBufferSize(buffer.Count - ++i, this);
                });
                buffer.Clear();
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
