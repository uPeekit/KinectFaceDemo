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
        // util

        public static string GetIncreasedVersionOfFile(string directory, string fileName)
        {
            string[] existingNumbers = Directory.GetFiles(directory).Where(file => new FileInfo(file).Name.StartsWith(fileName))
                                                                    .Select(file => file.Remove(0, directory.Length + fileName.Length).Split('.').First())
                                                                    .ToArray();
            if (existingNumbers.Length == 0) return fileName;

            int parsed;
            int maxNumber = Array.ConvertAll(existingNumbers,
                str => int.TryParse(str, out parsed) ? parsed : 0).Max();

            return fileName + (maxNumber + 1);
        }

        // read

        public static void ReadCsv(string filePath, 
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

        public static void ReadCsv(string filePath, Action<int, string> lineProcessor)
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
    }
}
