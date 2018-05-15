using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace KinectDemo
{
    public class ParametrizedConverter
    {
        public IParametrizedConvertStrategy ParametrizedConvertStrategy { get; set; }

        public string ConvertId { get; set; }

        public void Convert()
        {
            string resultDir = Constants.DIR_BASE_OUTPUT + Constants.DIR_CONVERTED;
            string resultFilePath = string.Format("{0}{1}.csv",
                resultDir,
                FilesHelper.GetIncreasedVersionOfFile(
                    resultDir,
                    string.Format("{0}_{1}", ParametrizedConvertStrategy.ResultFileName, ConvertId)));

            ParametrizedConvertStrategy.Parameters.ForEach(param => param.ProcessValue());
            var toWrite = ParametrizedConvertStrategy.GetResultDataWithHeaders();
            CsvHelper.WriteCsv(resultFilePath, toWrite[0], toWrite.Skip(1).ToList());
        }

    }
}