using System;
using System.Collections.Generic;

namespace KinectDemo
{
    class DataConverter
    {
        public ConvertStrategy ConvertStrategy { get; set; }

        public string ConvertId { private get; set; }

        public string[] FilesToConvert { private get; set; }

        List<NamedIndexedList> result;

        public void Convert(string parameters)
        {
            if (FilesToConvert == null || FilesToConvert.Length == 0)
                throw new Exception("no no no");

            result = ConvertStrategy.ConsumeFiles(parameters, FilesToConvert);

            string resultDir = Constants.DIR_BASE_OUTPUT + Constants.DIR_CONVERTED;
            string resultFilePath = string.Format("{0}{1}.csv", 
                resultDir, 
                FilesHelper.GetIncreasedVersionOfFile(
                    resultDir, 
                    string.Format("{0}_{1}", ConvertStrategy.ResultFileName, ConvertId)));
            
            FilesHelper.WriteLogLine(resultDir + Constants.LOG_FILE_NAME, ConvertStrategy.GetLogSummary(result));
            CsvHelper.WriteCsv(resultFilePath, result, ConvertStrategy.DoIndexesMatter());

            FilesToConvert = null;
        }

    }
}
