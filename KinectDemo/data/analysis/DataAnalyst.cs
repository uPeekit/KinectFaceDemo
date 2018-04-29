using KinectDemo.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace KinectDemo
{
    class DataAnalyst
    {
        public AnalysisStrategy analysisStrategy;

        public List<FileList> FileGroups { get; private set; } = new List<FileList>();
        private List<PointPositionsList> currentPoints = new List<PointPositionsList>();

        private string currentFile;
        private long prevTimestamp;

        public void Analyse()
        {
            Dictionary<string, List<double>> result = new Dictionary<string, List<double>>();
            foreach (var fileList in FileGroups)
            {
                ProcessFileList(fileList, result);
            }
            string resultFilePath = string.Format(@"{0}{1}{2}.csv",
                Constants.DIR_BASE_OUTPUT,
                Constants.DIR_RESULT,
                FilesHelper.GetIncreasedVersionOfFile(Constants.DIR_BASE_OUTPUT + Constants.DIR_RESULT, Constants.RESULT_FILE_NAME));
            CsvHelper.WriteCsv(
                resultFilePath,
                FileGroupsToHeaders(),
                result.Select(KeyValueToList).ToList());

            MessageBox.Show("Done!\n" + resultFilePath);
            FileGroups.Clear();
        }

        private List<string> FileGroupsToHeaders()
        {
            var list = new List<string>() { "" };
            list.AddRange(FileGroups.Select(fileList => new FileInfo(fileList.files[0]).Name.Split('.')[0] + "..").ToList());
            return list;
        }

        private List<string> KeyValueToList(KeyValuePair<string, List<double>> keyValuePair)
        {
            var list = new List<string>() { keyValuePair.Key };
            list.AddRange(keyValuePair.Value.Select(d => d.ToString()));
            return list;
        }

        private void ProcessFileList(FileList fileList, Dictionary<string, List<double>> result)
        {
            Dictionary<string, double> groupResult = new Dictionary<string, double>();
            fileList.ForEach(file =>
            {
                try { ProcessFile(file); }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return; // works like continue to outer loop
                }
                analysisStrategy.ConsumeFile(currentPoints);
            });
            groupResult = PointsHelper.ExtractNamedValues(analysisStrategy.GetResult());
            groupResult.ToList().ForEach(keyValuePair =>
            {
                if (result.ContainsKey(keyValuePair.Key))
                    result[keyValuePair.Key].Add(keyValuePair.Value);
                else
                    result.Add(keyValuePair.Key, new List<double>() { keyValuePair.Value });
            });
        }

        // moved to converter
        private void ProcessFile(string file)
        {
            prevTimestamp = 0;
            currentPoints.Clear();
            ParsePoints(currentFile = file);
        }

        private void ParsePoints(string file)
        {
            CsvHelper.ReadCsvByTokens(file,
                                      CheckTimestamp,
                                      ProcessTokenAndAddPoint);
        }

        private void CheckTimestamp(int lineNumber, string timestamp)
        {
            long parsed = long.Parse(timestamp);
            if (parsed < prevTimestamp)
                throw new Exception(currentFile + ": order is broken on line " + lineNumber);
            prevTimestamp = parsed;
        }

        private void ProcessTokenAndAddPoint(int lineNumber, string token)
        {
            int pointNumber = int.Parse(token.Split(' ')[0]);

            if (currentPoints.Count <= pointNumber + 1)
                currentPoints.Add(PointPositionsList.For(pointNumber));
            currentPoints[pointNumber].Add(ParsePoint(token));
        }

        private PointPositionsList.Position ParsePoint(string str)
        {
            var arr = str.Split(' ');
            return PointPositionsList.Position.Of(double.Parse(arr[1]), double.Parse(arr[2]), double.Parse(arr[3]));
        }
    }
}
