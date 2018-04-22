using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using static KinectDemo.MainWindow;
using KinectDemo.data;

namespace KinectDemo
{
    public class DataWriter
    {
        private const int BUFFER_SIZE = 50;

        private MainWindow mainWindow;
        private RecordMode recordMode;
        private ConcurrentQueue<IReadOnlyList<CameraSpacePoint>> buffer;
        private string fileName;
        private StreamWriter file;
        private Thread loop;
        private bool isActive;

        public DataWriter(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public void StartRecording()
        {
            if (!mainWindow.IsSensorOpen()) throw new Exception("No");
            if (!mainWindow.IsNameSet()) throw new Exception("Enter name");
            if (!mainWindow.IsLanguageSet()) throw new Exception("Enter language");
            if ((recordMode = mainWindow.GetRecordMode()) == null) throw new Exception("Choose mode");

            isActive = true;

            fileName = CreateNewRecordFile();
            mainWindow.SetFileName(fileName);

            buffer = new ConcurrentQueue<IReadOnlyList<CameraSpacePoint>>();
            file = new StreamWriter(recordMode.FullOutDir() + fileName, true, Encoding.ASCII, 1024);
            file.AutoFlush = true;

            loop = new Thread(Loop);
            loop.Start();
        }

        private string CreateNewRecordFile()
        {
            string fileName = CsvHelper.GetIncreasedVersionOfFile(recordMode.FullOutDir(), recordMode.ComposeFileName()) + ".csv";
            File.Create(recordMode.FullOutDir() + fileName).Dispose();
            return fileName;
        }

        private void Loop()
        {
            IReadOnlyList<CameraSpacePoint> points;
            try
            {
                while (isActive)
                {
                    if (!buffer.TryDequeue(out points))
                        Thread.Sleep(10);
                    else
                        WriteLine(points);
                }
            }
            catch (ThreadAbortException e) { }
            finally
            {
                while (buffer.TryDequeue(out points))
                    WriteLine(points);

                file.Close();
            }
        }
        
        private void WriteLine(IReadOnlyList<CameraSpacePoint> points)
        {
            CsvHelper.WriteCsvLine(file, points);
            mainWindow.SetBufferSize(buffer.Count, this);
        }

        public void EndRecording()
        {
            isActive = false;
            loop.Abort();
        }

        public void AddFaceData(IReadOnlyList<CameraSpacePoint> vertices)
        {
            if (!isActive) return;

            buffer.Enqueue(vertices);
            mainWindow.SetBufferSize(buffer.Count, this);
        }
    }
}
