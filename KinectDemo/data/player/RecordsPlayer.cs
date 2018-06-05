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
        public float speedFactor = 1;

        private MainWindow mainWindow;
        private List<Ellipse> _points = new List<Ellipse>();
        private int pIndex;
        private StreamReader stream;
        private long prevTimestamp;

        private CancellationTokenSource cancelTokenSource;
        private Task playTask;

        public RecordsPlayer(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public async void PlayFile()
        {
            CreatePointsIfNeeded();

            prevTimestamp = -1;
            pIndex = 0;
            stream = new StreamReader(filePath);

            cancelTokenSource = new CancellationTokenSource();
            playTask = PlayFrames(cancelTokenSource.Token);

            try
            {
                await playTask;
            }
            catch (Exception e) { }
            finally
            {
                try { playTask.Dispose(); } catch(Exception e) { }
                stream.Close();
            }
        }
        
        Ellipse point;

        private async Task PlayFrames(CancellationToken cancelToken)
        {
            long timestamp;
            while ((line = stream.ReadLine()) != null)
            {
                timestamp = long.Parse(line.Split(';').Take(1).ToList()[0]);
                if (prevTimestamp > 0) await Task.Delay((int)((timestamp - prevTimestamp) / speedFactor));
                //await Task.Delay(30);
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
            //return new DepthSpacePoint
            //{
            //    X = (toMap.X * 1000 + 100) + recX,
            //    Y = (toMap.Y * 1000 + 100) + recY
            //};
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
                List<double> fscores = CsvHelper.ParseFileToNamedIndexedLists("C:\\KinectData\\converted\\f_scores.csv", true, ',')[0].OnlyValues();
                double maxscore = fscores.Max();
                List<Color> gradient = CreateGradient(Colors.Yellow, Colors.Red);
                bool special;
                for (int index = 0; index < POINTS_COUNT; index++)
                {
                    special = new List<int> { 3529, 3015 }.Select(n => n % 1347).Contains(index);
                    Ellipse ellipse = new Ellipse
                    {
                        Width = special ? 6 : 2,
                        Height = special ? 6 : 2,
                        Fill = new SolidColorBrush(special ? Colors.LightBlue : gradient[(int)(fscores[index] / maxscore * 100)])
                    };
                    _points.Add(ellipse);
                    mainWindow.AddPlayerPoint(ellipse);
                }
            }
        }

        private List<Color> CreateGradient(Color from, Color to)
        {
            int size = 101;
            int rMax = from.R;
            int rMin = to.R;
            int gMax = from.G;
            int gMin = to.G;
            int bMax = from.B;
            int bMin = to.B;

            var colorList = new List<Color>();
            for (int i = 0; i < size; i++)
            {
                var rAverage = rMin + (rMax - rMin) * i / size;
                var gAverage = gMin + (gMax - gMin) * i / size;
                var bAverage = bMin + (bMax - bMin) * i / size;
                colorList.Add(Color.FromRgb((byte)rAverage, (byte)gAverage, (byte)bAverage));
            }
            return colorList;
        }
        
    }
}
