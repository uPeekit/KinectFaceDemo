using System.Linq;
using System.IO;
using System.Collections.Generic;
using System;
using System.Windows;
using KinectDemo.data;

namespace KinectDemo
{
    public interface AnalysisStrategy
    {
        void AddFilePoints(List<PointList> points);

        Dictionary<string, double> Process();
    }

    public class DataAnalyst
    {
        private MainWindow mainWindow;
        private AnalysisStrategy analysisStrategy;

        private List<FileList> fileGroups = new List<FileList>();
        private List<PointList> currentPoints = new List<PointList>();

        private string currentFile;
        private long prevTimestamp;

        public DataAnalyst(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.mainWindow.SetGroupsList(fileGroups);
        }

        public void Analyse()
        {
            if (fileGroups.Count == 0) return;

            Type strategyType = Type.GetType("KinectDemo." + mainWindow.GetChosenAnalysisOption());
            analysisStrategy = (AnalysisStrategy)Activator.CreateInstance(strategyType, Constants.DIR_BASE_OUTPUT);
            
            Dictionary<string, List<double>> result = new Dictionary<string, List<double>>();
            foreach (var fileList in fileGroups)
            {
                ProcessFileList(fileList, result);
            }
            string resultFilePath = string.Format(@"{0}{1}{2}.csv", 
                Constants.DIR_BASE_OUTPUT,
                Constants.DIR_RESULT,
                CsvHelper.GetIncreasedVersionOfFile(Constants.DIR_BASE_OUTPUT + Constants.DIR_RESULT, Constants.RESULT_FILE_NAME));
            CsvHelper.WriteCsv(
                resultFilePath,
                FileGroupsToHeaders(),
                result.Select(KeyValueToList).ToList());

            MessageBox.Show("Done!\n" + resultFilePath);
            fileGroups.Clear();
        }

        private void ProcessFileList(FileList fileList, Dictionary<string, List<double>> result)
        {
            Dictionary<string, double> groupResult = new Dictionary<string, double>();
            fileList.Iterate(file =>
            {
                try { ProcessFile(file); }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return; // works like continue to outer loop
                }
                analysisStrategy.AddFilePoints(currentPoints);
            });
            groupResult = analysisStrategy.Process();
            groupResult.ToList().ForEach(keyValuePair =>
            {
                if (result.ContainsKey(keyValuePair.Key))
                    result[keyValuePair.Key].Add(keyValuePair.Value);
                else
                    result.Add(keyValuePair.Key, new List<double>() { keyValuePair.Value });
            });
        }

        private void ProcessFile(string file)
        {
            currentFile = file;
            prevTimestamp = 0;
            currentPoints.Clear();
            ParsePoints(file);
        }

        private List<string> FileGroupsToHeaders()
        {
            var list = new List<string>() { "" };
            list.AddRange(fileGroups.Select(fileList => new FileInfo(fileList.files[0]).Name.Split('.')[0] + "..").ToList());
            return list;
        }

        private List<string> KeyValueToList(KeyValuePair<string, List<double>> keyValuePair)
        {
            var list = new List<string>() { keyValuePair.Key };
            list.AddRange(keyValuePair.Value.Select(d => d.ToString()));
            return list;
        }
        
        public void AddGroup()
        {
            string[] files = GetChosenFiles();
            if (files == null || files.Length == 0)
                return;

            fileGroups.Add(FileList.Of(files));
            mainWindow.RefreshListView();
        }

        public void RemoveGroup(FileList fileList)
        {
            fileGroups.Remove(fileList);
            mainWindow.RefreshListView();
        }

        private string[] GetChosenFiles()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".csv";
            dlg.InitialDirectory = Constants.DIR_BASE_OUTPUT;
            dlg.Multiselect = true;

            var result = dlg.ShowDialog();
            return result.HasValue && result.Value ? dlg.FileNames : null;
        }

        private void ParsePoints(string file)
        {
            CsvHelper.ReadCsv(file, 
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
                currentPoints.Add(new PointList());
            currentPoints[pointNumber].Add(ParsePoint(token));
        }

        private PointList.Point ParsePoint(string str)
        {
            var arr = str.Split(' ');
            return PointList.Point.Of(double.Parse(arr[1]), double.Parse(arr[2]), double.Parse(arr[3]));
        }
    }

}