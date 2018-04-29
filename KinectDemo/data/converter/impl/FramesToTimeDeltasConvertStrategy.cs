using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KinectDemo
{
    public class FramesToTimeDeltasConvertStrategy : ConvertStrategy
    {
        private long delta, timestamp, lastTimestamp;

        public string ResultFileName { get; set; } = Constants.DIR_TIME_DELTAS;

        public string CurrentFile { get; set; }

        private List<double> currentResult;
        public List<double> CurrentResult {
            get { return ResetFieldsAndGetResult(); }
            set { currentResult = value; } }

        public FramesToTimeDeltasConvertStrategy()
        {
            ResetFieldsAndGetResult();
        }

        public List<double> ConsumeFile(string file)
        {
            CsvHelper.ReadCsvByLines(file, AddDataLine);
            return CurrentResult;
        }

        private void AddDataLine(int lineNumber, string line)
        {
            timestamp = long.Parse(line.Substring(0, 14));
            if (lastTimestamp > 0)
            {
                delta = timestamp - lastTimestamp;
                CurrentResult.Add(delta);
            }
            lastTimestamp = timestamp;
        }

        private List<double> ResetFieldsAndGetResult()
        {
            lastTimestamp = -1;
            return currentResult;
        }

        public string GetLogSummary(string filePath, List<double> res)
        {
            return string.Format("{0}\t {1,-35} min: {2}, max {3}, avg: {4}", 
                DateTime.Now, new FileInfo(filePath).Name, res.Min(), res.Max(), res.Average());
        }

        public bool DoIndexesMatter()
        {
            return false;
        }
        
    }
}
