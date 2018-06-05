using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KinectDemo
{
    class CsvHelper
    {
        // read

        public static void ReadCsvByTokens(string filePath, 
                                           Action<int, string> firstTokenProcessor, 
                                           Action<int, string> anyTokenProcessor) {
            string line;
            bool isFirstToken;
            IEnumerable<string> iter;

            using (var stream = new StreamReader(filePath))
            {
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
            }
        }

        public static void ReadCsvByLines(string filePath, Action<int, string> lineProcessor)
        {
            string line;
            using (var stream = new StreamReader(filePath))
            {
                var lineNumber = -1;
                while ((line = stream.ReadLine()) != null)
                    lineProcessor(++lineNumber, line);
            }
        }

        // write

        public static void WriteCsv(string filePath, List<string> headers, List<List<string>> data)
        {
            using (var stream = new StreamWriter(filePath))
            {
                if(headers != null)
                stream.WriteLineAsync(AggrListToCsvString(headers)).Wait();
                List<string> line;
                for (var i = 0; i < data.Count; ++i)
                {
                    line = data[i];
                    stream.WriteLineAsync(AggrListToCsvString(line)).Wait();
                }
            }
        }

        private static string AggrListToCsvString(List<string> list) => 
            list.Aggregate((str1, str2) => string.Join(Constants.CSV_SEPARATOR.ToString(), str1, str2));

        // points
        // write

        public static void WriteCameraPointsAsCsvLine(StreamWriter file, IReadOnlyList<CameraSpacePoint> points)
        {
            file.WriteLineAsync(ConvertCameraPointsListToString(points)).Wait();
        }

        private static string ConvertCameraPointsListToString(IReadOnlyList<CameraSpacePoint> vertices)
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
            long currentTimestamp = 0;
            ReadCsvByTokens(file,
                            (n, token) => currentTimestamp = long.Parse(token),
                            (n, line) => ProcessTokenAndAddPoint(points, line, currentTimestamp));
            return points;
        }

        private static void ProcessTokenAndAddPoint(List<PointPositionsList> points, string token, long timestamp)
        {
            int pointNumber = int.Parse(token.Split(' ')[0]);

            if (points.Count <= pointNumber + 1)
                points.Add(PointPositionsList.For(pointNumber));
            points[pointNumber].Add(ParsePoint(token));
            points[pointNumber].timestamps.Add(timestamp);
        }

        private static PointPositionsList.Position ParsePoint(string str)
        {
            var arr = str.Split(' ');
            return PointPositionsList.Position.Of(double.Parse(arr[1]), double.Parse(arr[2]), double.Parse(arr[3]));
        }

        // converted result
        // write

        public static void WriteCsv(string resultFilePath, List<NamedIndexedList> data, bool doIndexesMatter)
        {
            List<List<string>> resultData = new List<List<string>>();
            List<string> headers = new List<string>();
            List<string> currentValuesToWrite;

            List<int> indexes = data[0].IndexedValues.Select(kvPair => kvPair.Key).ToList();
            if (doIndexesMatter) headers.Add("index");

            foreach (var fileWithValues in data)
            {
                headers.Add(fileWithValues.Name);
                currentValuesToWrite = fileWithValues.OnlyValues().Select(val => val.ToString()).ToList();
                for (var currentValueIndex = 0; currentValueIndex < currentValuesToWrite.Count; ++currentValueIndex)
                {
                    if (resultData.Count <= currentValueIndex)
                    {
                        resultData.Add(new List<string>());
                        if (doIndexesMatter) resultData[currentValueIndex].Add(indexes[currentValueIndex].ToString());

                    }

                    resultData[currentValueIndex].Add(currentValuesToWrite[currentValueIndex]);
                }
            }
            WriteCsv(resultFilePath, headers, resultData);
        }

        // read

        public static List<NamedIndexedList> ParseFileToNamedIndexedLists(string fileToAnalyse, bool firstIndexColumn)
        {
            return ParseFileToNamedIndexedLists(fileToAnalyse, firstIndexColumn, Constants.CSV_SEPARATOR);
        }

        public static List<NamedIndexedList> ParseFileToNamedIndexedLists(string fileToAnalyse, bool firstIndexColumn, char separator)
        {
            List<NamedIndexedList> result = new List<NamedIndexedList>();
            using (var stream = new StreamReader(fileToAnalyse))
            {
                string line;
                List<string> lineTokens;
                bool firstLine = true;
                int valueIndex;
                string token;
                var i = -2;
                while ((line = stream.ReadLine()) != null)
                {
                    i++;
                    lineTokens = line.Split(separator).ToList();
                    for (var columnIndex = 0; columnIndex < lineTokens.Count; ++columnIndex)
                    {
                        if(firstIndexColumn && columnIndex == 0)
                            continue;

                        token = lineTokens[columnIndex];
                        if (firstLine)
                        {
                            result.Add(new NamedIndexedList(token));
                        }
                        else
                        {
                            valueIndex = firstIndexColumn ? int.Parse(lineTokens[0]) : i;
                            if (token.Length > 0)
                                result[firstIndexColumn ? columnIndex-1 : columnIndex].IndexedValues.Add(valueIndex, double.Parse(token));
                        }
                    }
                    firstLine = false;
                }
            }
            return result;
        }

    }
}
