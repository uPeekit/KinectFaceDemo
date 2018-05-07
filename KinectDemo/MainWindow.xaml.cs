using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;

namespace KinectDemo
{
    public partial class MainWindow : Window
    {
        // Provides a Kinect sensor reference.
        private KinectSensor _sensor;

        private MultiSourceFrameReader _reader;

        // Acquires body frame data.
        private BodyFrameSource _bodySource;

        // Reads body frame data.
        private BodyFrameReader _bodyReader;

        // Acquires HD face data.
        private HighDefinitionFaceFrameSource _faceSource;

        // Reads HD face data.
        private HighDefinitionFaceFrameReader _faceReader;

        // Required to access the face vertices.
        private FaceAlignment _faceAlignment;

        // Required to access the face model points.
        private FaceModel _faceModel;

        // Used to display 1,000 points on screen.
        private List<Ellipse> _points = new List<Ellipse>();

        private Boolean record = false;
        private DataWriter dataWriter;
        private DataAnalysisHandler dataAnalysisHandler;
        private RecordsPlayer recordsPlayer;
        private DataConvertHandler dataConvertHandler;

        public MainWindow()
        {
            InitializeComponent();
            dataAnalysisHandler = new DataAnalysisHandler(this);
            dataConvertHandler = new DataConvertHandler(this);
            recordsPlayer = new RecordsPlayer(this);
            CreateFoldersIfNeeded();
            InitAnalysisStrategies();
            InitConvertStrategies();
        }

        private void CreateFoldersIfNeeded()
        {
            Directory.CreateDirectory(Constants.DIR_BASE_OUTPUT);
            Directory.CreateDirectory(Constants.MODE_NATIVE.FullOutDir());
            Directory.CreateDirectory(Constants.MODE_FOREIGN.FullOutDir());
            Directory.CreateDirectory(Constants.DIR_BASE_OUTPUT + Constants.DIR_CONVERTED);
            Directory.CreateDirectory(Constants.DIR_BASE_OUTPUT + Constants.DIR_ANALYSED);
        }

        private void InitAnalysisStrategies()
        {
            AnalysisType.ItemsSource = GetImplementations(typeof(IAnalysisStrategy));
            AnalysisType.SelectedIndex = 0;
        }

        private void InitConvertStrategies()
        {
            ConvertType.ItemsSource = GetImplementations(typeof(ConvertStrategy));
            ConvertType.SelectedIndex = 0;
        }

        private IEnumerable<string> GetImplementations(Type type)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                                          .SelectMany(s => s.GetTypes())
                                          .Where(p => type.IsAssignableFrom(p) && !type.Equals(p) && !p.IsAbstract)
                                          .Select(t => t.Name);
        }

        private void ExecuteCatchingException(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "Oops");
            }
        }

        public void ShowPopupDone()
        {
            MessageBox.Show("Done!", "Yass");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _sensor = KinectSensor.GetDefault();

            if (_sensor != null)
            {
                // Listen for body data.
                _bodySource = _sensor.BodyFrameSource;
                _bodyReader = _bodySource.OpenReader();
                _bodyReader.FrameArrived += BodyReader_FrameArrived;

                // Listen for HD face data.
                _faceSource = new HighDefinitionFaceFrameSource(_sensor);
                _faceReader = _faceSource.OpenReader();
                _faceReader.FrameArrived += FaceReader_FrameArrived;

                _faceModel = new FaceModel();
                _faceAlignment = new FaceAlignment();

                // color camera
                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Depth);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
            }
        }

        private void BodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    Body[] bodies = new Body[frame.BodyCount];
                    frame.GetAndRefreshBodyData(bodies);

                    Body body = bodies.Where(b => b.IsTracked).FirstOrDefault();

                    if (!_faceSource.IsTrackingIdValid)
                    {
                        if (body != null)
                        {
                            _faceSource.TrackingId = body.TrackingId;
                        }
                    }
                }
            }
        }

        private void FaceReader_FrameArrived(object sender, HighDefinitionFaceFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null && frame.IsFaceTracked)
                {
                    frame.GetAndRefreshFaceAlignmentResult(_faceAlignment);
                    UpdateFacePoints();
                }
            }
        }

        private void UpdateFacePoints()
        {
            if (_faceModel == null) return;

            var vertices = _faceModel.CalculateVerticesForAlignment(_faceAlignment);

            if (vertices.Count > 0)
            {
                if(record)
                {
                    dataWriter.AddFaceData(vertices);
                }
                if (_points.Count == 0)
                {
                    for (int index = 0; index < vertices.Count; index++)
                    {
                        Ellipse ellipse = new Ellipse
                        {
                            Width = 1.0,
                            Height = 1.0,
                            Fill = new SolidColorBrush(Colors.Red)
                        };

                        _points.Add(ellipse);
                    }

                    foreach (Ellipse ellipse in _points)
                    {
                        canvas.Children.Add(ellipse);
                    }
                }

                for (int index = 0; index < vertices.Count; index++)
                {
                    CameraSpacePoint vertice = vertices[index];
                    DepthSpacePoint point = _sensor.CoordinateMapper.MapCameraPointToDepthSpace(vertice);

                    if (float.IsInfinity(point.X) || float.IsInfinity(point.Y)) return;

                    Ellipse ellipse = _points[index];

                    Canvas.SetLeft(ellipse, point.X);
                    Canvas.SetTop(ellipse, point.Y);
                }
            }
        }

        private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e) {
            // Get a reference to the multi-frame
            var reference = e.FrameReference.AcquireFrame();

            // Open color frame
            using (var frame = reference.DepthFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    camera.Source = ToBitmap(frame);
                }
            }
        }

        private ImageSource ToBitmap(DepthFrame frame)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;

            ushort minDepth = frame.DepthMinReliableDistance;
            ushort maxDepth = frame.DepthMaxReliableDistance;

            ushort[] depthData = new ushort[width * height];
            byte[] pixelData = new byte[width * height * (PixelFormats.Bgr32.BitsPerPixel + 7) / 8];

            frame.CopyFrameDataToArray(depthData);

            int colorIndex = 0;
            for (int depthIndex = 0; depthIndex < depthData.Length; ++depthIndex)
            {
                ushort depth = depthData[depthIndex];
                byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);

                pixelData[colorIndex++] = intensity; // Blue
                pixelData[colorIndex++] = intensity; // Green
                pixelData[colorIndex++] = intensity; // Red

                ++colorIndex;
            }

            int stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixelData, stride);
        }

    }
}
