using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System;
using System.Windows;
using Microsoft.Kinect.Face;

namespace KinectDemo
{
    public interface AnalysisStrategy
    {
        void AddFilePoints(List<List<Tuple<double, double, double>>> points);
        Dictionary<string, double> Process();
    }

    public class FileList
    {
        public string[] files { get; private set; }

        private FileList(string[] files) { this.files = files; }

        public static FileList Of(string[] files) { return new FileList(files); }

        public override string ToString()
        {
            return files.Length == 1 
                ? files[0].Split('\\').Last()
                : files.Aggregate((str1, str2) => string.Join(", ", new FileInfo(str1).Name, new FileInfo(str2).Name));
        }
    }

    public class DataAnalyst
    {
        private List<List<Tuple<double, double, double>>> currentPoints = new List<List<Tuple<double, double, double>>>();
        private string currentFile;
        private long prevTimestamp;

        private MainWindow mainWindow;
        private AnalysisStrategy analysisStrategy;

        private List<FileList> fileGroups = new List<FileList>();

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
                FileGroupsToHeaderList(),
                result.Select(KeyValueToList).ToList());

            MessageBox.Show("Done!\n" + resultFilePath);
            fileGroups.Clear();
        }

        private void ProcessFileList(FileList fileList, Dictionary<string, List<double>> result)
        {
            Dictionary<string, double> singleResult = new Dictionary<string, double>();
            foreach (var file in fileList.files)
            {
                try { ProcessFile(file); }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                analysisStrategy.AddFilePoints(currentPoints);
            }
            singleResult = analysisStrategy.Process();
            singleResult.ToList().ForEach(keyValuePair =>
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

        private List<string> FileGroupsToHeaderList()
        {
            var list = new List<string>() { "" };
            list.AddRange(fileGroups.Select(fileList => new FileInfo(fileList.files[0]).Name.Split('.')[0] + "...").ToList());
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
                              SetScale,
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

        private void SetScale(int lineNumber, IEnumerable<string> lineIter)
        {
            if (currentPoints.Count == 0)
                currentPoints.Add(new List<Tuple<double, double, double>>());
            currentPoints[0].Add(GetScaleDist(lineIter));
        }

        private Tuple<double, double, double> GetScaleDist(IEnumerable<string> lineIter)
        {
            var right = StrToPoint(lineIter.ElementAt((int)HighDetailFacePoints.RighteyeOutercorner + 1));
            var left = StrToPoint(lineIter.ElementAt((int)HighDetailFacePoints.LefteyeOutercorner + 1));

            return new Tuple<double,double,double>(
                Math.Sqrt(Math.Pow(right.Item1 - left.Item1, 2) + Math.Pow(right.Item2 - left.Item2, 2) + Math.Pow(right.Item3 - left.Item3, 2)), 0, 0);
        }

        private void ProcessTokenAndAddPoint(int lineNumber, string token)
        {
            int pointNumber = int.Parse(token.Split(' ')[0]);

            if (currentPoints.Count <= pointNumber + 1)
                currentPoints.Add(new List<Tuple<double, double, double>>());
            currentPoints[pointNumber + 1].Add(StrToPoint(token));
        }

        private Tuple<double, double, double> StrToPoint(string str)
        {
            var arr = str.Split(' ');
            return new Tuple<double, double, double>(double.Parse(arr[1]), double.Parse(arr[2]), double.Parse(arr[3]));
        }
    }

}