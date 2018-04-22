using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FaceRecordsPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int POINTS_COUNT = 1347;

        private string filePath;
        private List<Ellipse> _points = new List<Ellipse>();
        private int pIndex;
        private StreamReader stream;

        public MainWindow()
        {
            InitializeComponent();
            filePath = @"C:\KinectData\native\n_eng_ethan.csv";
        }

        private void ChooseFile(object sender, RoutedEventArgs e)
        {
            filePath = PromptChooseFile();
        }

        private string PromptChooseFile()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".csv",
                InitialDirectory = @"C:\KinectData\",
            };

            var result = dlg.ShowDialog();
            return result.HasValue && result.Value ? dlg.FileName : null;
        }

        private void PlayFile(object sender, RoutedEventArgs e)
        {
            CreatePointsIfNeeded();

            pIndex = 0;
            stream = new StreamReader(filePath);
            PlayFrames();
            //stream.Close();
        }

        private async void PlayFrames()
        {
            while ((line = stream.ReadLine()) != null)
            {
                foreach (var token in line.Split(';').Skip(1))
                {
                    DepthSpacePoint mapped = await MapTokenToPoint(token);
                    if (float.IsInfinity(mapped.X) || float.IsInfinity(mapped.Y) || mapped.X < 0 || mapped.Y < 0) continue;

                    Dispatcher.Invoke(new Action(() =>
                    {
                        Canvas.SetLeft(point, mapped.X);
                        Canvas.SetTop(point, mapped.Y);
                    }));
                }
                await Task.Delay(10);
            }
        }

        string line;
        string[] tokenArr;
        Ellipse point;

        private async Task<DepthSpacePoint> MapTokenToPoint(string token)
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
                X = toMap.X * 600f / -0.8f + 300f,
                Y = toMap.Y * 600f / -0.8f + 260f
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
                        Width = 1.0,
                        Height = 1.0,
                        Fill = new SolidColorBrush(Colors.Red)
                    };
                    _points.Add(ellipse);
                    canvas.Children.Add(ellipse);
                }
            }
        }
    }
}
