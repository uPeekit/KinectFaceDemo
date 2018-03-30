using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Windows;
using static KinectDemo.MainWindow;
using Microsoft.Kinect.Face;

namespace KinectDemo
{
    public class DataWriter
    {
        private const int BUFFER_SIZE = 50;

        private MainWindow mainWindow;
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
            if (mainWindow.GetMode() == Mode.NONE) throw new Exception("Choose mode");
            if (mainWindow.GetName().Length == 0) throw new Exception("Enter name");
            if (!mainWindow.IsSensorOpen()) throw new Exception("No");

            isActive = true;

            string outputDir = GetOutputDirectory();
            fileName = CreateNewRecordFile(outputDir);
            mainWindow.SetFileName(fileName);

            buffer = new ConcurrentQueue<IReadOnlyList<CameraSpacePoint>>();
            file = new StreamWriter(outputDir + fileName, true, Encoding.ASCII, 1024);
            file.AutoFlush = true;

            loop = new Thread(Loop);
            loop.Start();
        }

        private string GetOutputDirectory()
        {
            return Constants.DIR_BASE_OUTPUT + (mainWindow.GetMode() == Mode.NATIVE ? Constants.DIR_NATIVE : Constants.DIR_FOREIGN);
        }

        private string CreateNewRecordFile(string outputDir)
        {
            string fileName = CsvHelper.GetIncreasedVersionOfFile(outputDir, ComposeFileNameFromUiInput()) + ".csv";
            File.Create(outputDir + fileName).Dispose();
            return fileName;
        }

        private string ComposeFileNameFromUiInput()
        {
            return (mainWindow.GetMode() == Mode.NATIVE ? Constants.PREFIX_NATIVE : Constants.PREFIX_FOREIGN) + mainWindow.GetName();
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
