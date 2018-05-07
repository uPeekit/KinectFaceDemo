using System.Collections.Generic;

namespace KinectDemo
{
    class DataAnalyst
    {
        public IAnalysisStrategy analysisStrategy;

        public string[] FilesToAnalyse { get; set; }

        public string AnalyseId { private get; set; }

        private List<NamedIndexedList> result;

        public void Analyse(string parameters)
        {
            result = analysisStrategy.ConsumeFilesConverted(parameters,  FilesToAnalyse);

            string resultFilePath = string.Format("{0}{1}{2}.csv",
                Constants.DIR_BASE_OUTPUT,
                Constants.DIR_ANALYSED,
                FilesHelper.GetIncreasedVersionOfFile(
                    Constants.DIR_BASE_OUTPUT + Constants.DIR_ANALYSED, 
                    string.Format("{0}_{1}", analysisStrategy.ResultFileName, AnalyseId)));

            CsvHelper.WriteCsv(resultFilePath, result, false);
        }
    }
}
