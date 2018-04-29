using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KinectDemo
{
    class RecordsPlayer
    {
        private const int POINTS_COUNT = 1347;

        public string filePath;
        public float recX = 260f;
        public float recY = 150f;

        private MainWindow mainWindow;
        private List<Ellipse> _points = new List<Ellipse>();
        private int pIndex;
        private StreamReader stream;
        private long prevTimestamp = -1;

        private CancellationTokenSource cancelTokenSource;
        private Task task;

        public RecordsPlayer(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public async void PlayFile()
        {
            CreatePointsIfNeeded();

            pIndex = 0;
            stream = new StreamReader(filePath);

            cancelTokenSource = new CancellationTokenSource();
            task = PlayFrames(cancelTokenSource.Token);

            try
            {
                await task;
            }
            catch (Exception e) { }
            finally
            {
                try { task.Dispose(); } catch(Exception e) { }
                stream.Close();
            }
        }
        
        Ellipse point;

        private async Task PlayFrames(CancellationToken cancelToken)
        {
            long timestamp;
            while ((line = stream.ReadLine()) != null)
            {
                timestamp = long.Parse(line.Substring(0, 14));
                //if(prevTimestamp > 0) await Task.Delay((int)(timestamp - prevTimestamp)/2);
                await Task.Delay(30);
                prevTimestamp = timestamp;

                foreach (var token in line.Split(';').Skip(1))
                {
                    DepthSpacePoint mapped = MapTokenToPoint(token);
                    if (float.IsInfinity(mapped.X) || float.IsInfinity(mapped.Y) || mapped.X < 0 || mapped.Y < 0) continue;

                    mainWindow.DisplayPoint(point, mapped);
                }
                cancelToken.ThrowIfCancellationRequested();
            }
        }

        public void StopPlayingFile()
        {
            cancelTokenSource.Cancel();
        }

        string line;
        string[] tokenArr;

        private DepthSpacePoint MapTokenToPoint(string token)
        {
            tokenArr = token.Split(' ');
            pIndex = int.Parse(tokenArr[0]);
            point = _points[pIndex];

            CameraSpacePoint toMap = new CameraSpacePoint
            {
                X = float.Parse(tokenArr[1]),
                Y = float.Parse(tokenArr[2]),
                Z = float.Parse(tokenArr[3])
            };
            return MapCameraPoint(toMap);
        }

        private DepthSpacePoint MapCameraPoint(CameraSpacePoint toMap)
        {
            return new DepthSpacePoint()
            {
                X = toMap.X * 600f / -0.4f + recX,
                Y = toMap.Y * 600f / -0.4f + recY
            };
        }

        private void CreatePointsIfNeeded()
        {
            if (_points.Count == 0)
            {
                for (int index = 0; index < POINTS_COUNT; index++)
                {
                    Ellipse ellipse = new Ellipse
                    {
                        Width = 1.5,
                        Height = 1.5,
                        Fill = new SolidColorBrush(Colors.Red)
                    };
                    _points.Add(ellipse);
                    mainWindow.AddPlayerPoint(ellipse);
                }
            }
        }
        
    }
}
