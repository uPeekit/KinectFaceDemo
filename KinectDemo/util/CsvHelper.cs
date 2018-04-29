using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDemo
{
    class CsvHelper
    {
        // read

        public static void ReadCsvByTokens(string filePath,
                                           Action<int, string> tokenProcessor)
        {
            ReadCsvByTokens(filePath, null, tokenProcessor);
        }

        public static void ReadCsvByTokens(string filePath, 
                                           Action<int, string> firstTokenProcessor, 
                                           Action<int, string> anyTokenProcessor) {
            string line;
            bool isFirstToken;
            IEnumerable<string> iter;

            var stream = new StreamReader(filePath);
            var lineNumber = -1;
            while ((line = stream.ReadLine()) != null)
            {
                ++lineNumber;
                isFirstToken = true;
                iter = line.Split(Constants.CSV_SEPARATOR);

                foreach (var token in iter)
                {
                    if (firstTokenProcessor != null && isFirstToken)
                    {
                        isFirstToken = false;
                        firstTokenProcessor(lineNumber, token);
                        continue;
                    }
                    anyTokenProcessor(lineNumber, token);
                }
            }
            stream.Close();
        }
       
        public static void ReadCsvByLines(string filePath, Action<int, string> lineProcessor)
        {
            string line;
            var stream = new StreamReader(filePath);
            var lineNumber = -1;
            while ((line = stream.ReadLine()) != null)
            {
                lineProcessor(++lineNumber, line);
            }
            stream.Close();
        }

        // write

        public static void WriteCsv(string filePath, List<string> headers, List<List<string>> data)
        {
            var stream = new StreamWriter(filePath);
            stream.WriteLineAsync(headers.Aggregate((str1, str2) => string.Join(Constants.CSV_SEPARATOR.ToString(), str1, str2))).Wait();
            data.ForEach(line => stream.WriteLineAsync(line.Aggregate((str1, str2) => string.Join(Constants.CSV_SEPARATOR.ToString(), str1, str2))).Wait());
            stream.Close();
        }

        // points

        // write

        public static void WriteCsvLine(StreamWriter file, IReadOnlyList<CameraSpacePoint> points)
        {
            file.WriteLineAsync(ConvertPointsListToString(points)).Wait();
        }

        private static string ConvertPointsListToString(IReadOnlyList<CameraSpacePoint> vertices)
        {
            string result = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString();
            for (var i = 0; i < vertices.Count; ++i)
            {
                result += Constants.CSV_SEPARATOR.ToString() + ConvertPointToString(i, vertices[i]);
            }
            return result;
        }

        private static string ConvertPointToString(int n, CameraSpacePoint point)
        {
            return string.Format("{0} {1} {2} {3}", n.ToString(), point.X, point.Y, point.Z);
        }

        // read

        public static List<PointPositionsList> ParseFileToPointsPositions(string file)
        {
            List<PointPositionsList> points = new List<PointPositionsList>();
            ReadCsvByTokens(file,
                            (i, s) => { /* do nothing with first token - it is timestamp */ },
                            (n, line) => ProcessTokenAndAddPoint(points, n, line));
            return points;
        }

        private static void ProcessTokenAndAddPoint(List<PointPositionsList> points, int lineNumber, string token)
        {
            int pointNumber = int.Parse(token.Split(' ')[0]);

            if (points.Count <= pointNumber + 1)
                points.Add(PointPositionsList.For(pointNumber));
            points[pointNumber].Add(ParsePoint(token));
        }

        private static PointPositionsList.Position ParsePoint(string str)
        {
            var arr = str.Split(' ');
            return PointPositionsList.Position.Of(double.Parse(arr[1]), double.Parse(arr[2]), double.Parse(arr[3]));
        }

    }
}
