using KinectDemo.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KinectDemo
{
    class DataConverter
    {
        public ConvertStrategy ConvertStrategy { get; set; }

        public string[] Files { private get; set; }

        public void Convert()
        {
            if (Files == null || Files.Length == 0)
                throw new Exception("no no no");

            string resultDir = Constants.DIR_BASE_OUTPUT + Constants.DIR_CONVERTED;
            string resultFile = resultDir + FilesHelper.GetIncreasedVersionOfFile(resultDir, ConvertStrategy.ResultFileName) + ".csv";
            Directory.CreateDirectory(resultDir);

            string currentFile, currentFileName;
            List<double> currentResult;
            List<string> headers = new List<string>();
            List<List<double>> resultColumns = new List<List<double>>();

            for (var i = 0; i < Files.Length; ++i)
            {
                currentFile = Files[i];
                currentFileName = new FileInfo(currentFile).Name;
                currentResult = ConvertStrategy.ConsumeFile(currentFile);

                if (i == 0 && ConvertStrategy.DoIndexesMatter()) headers.Add("index");
                headers.Add(currentFileName);

                resultColumns.Add(currentResult);
                FilesHelper.WriteLogLine(resultDir + Constants.LOG_FILE_NAME, ConvertStrategy.GetLogSummary(currentFile, currentResult));
            }
            CsvHelper.WriteCsv(resultFile, headers, ListsHelper.ValuesColumnsToDataLines(resultColumns, ConvertStrategy.DoIndexesMatter()));
        }

    }
}
