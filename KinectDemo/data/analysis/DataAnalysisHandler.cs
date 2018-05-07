using System;

namespace KinectDemo
{
    public class DataAnalysisHandler
    {
        private MainWindow mainWindow;
        private DataAnalyst dataAnalyst = new DataAnalyst();

        public DataAnalysisHandler(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            mainWindow.SetAnalyseFiles(null);
        }

        public void Analyse()
        {
            if (dataAnalyst.FilesToAnalyse == null || dataAnalyst.FilesToAnalyse.Length == 0)
                throw new Exception("file.....!");
            if ((dataAnalyst.AnalyseId = mainWindow.GetAnalyseId()).Length == 0)
                throw new Exception("set id");

            dataAnalyst.Analyse(null);
        }

        public void ResetFile()
        {
            dataAnalyst.FilesToAnalyse = new string[] { };
        }

        public void ChooseFile()
        {
            string[] file = dataAnalyst.FilesToAnalyse = FilesHelper.PromptChooseFiles();
            mainWindow.SetAnalyseFiles(file);
        }

        public void SetAnalysisStrategy(string strategyName)
        {
            Type strategyType = Type.GetType("KinectDemo." + strategyName);
            dataAnalyst.analysisStrategy = (IAnalysisStrategy)Activator.CreateInstance(strategyType);
        }
    }
}