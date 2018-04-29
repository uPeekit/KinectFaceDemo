using System;
using KinectDemo.util;

namespace KinectDemo
{
    public class DataAnalysisHandler
    {
        private MainWindow mainWindow;
        private DataAnalyst dataAnalyst = new DataAnalyst();

        public DataAnalysisHandler(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.mainWindow.SetGroupsList(dataAnalyst.FileGroups);
        }

        public void Analyse()
        {
            if (dataAnalyst.FileGroups.Count == 0) return;

            Type strategyType = Type.GetType("KinectDemo." + mainWindow.GetChosenAnalysisOption());
            dataAnalyst.analysisStrategy = (AnalysisStrategy)Activator.CreateInstance(strategyType);

            dataAnalyst.Analyse();
        }

        public void AddGroup()
        {
            string[] files = FilesHelper.PromptChooseFiles();
            if (files == null || files.Length == 0)
                return;

            dataAnalyst.FileGroups.Add(FileList.Of(files));
            mainWindow.RefreshGroupsListView();
        }

        public void RemoveGroup(FileList fileList)
        {
            dataAnalyst.FileGroups.Remove(fileList);
            mainWindow.RefreshGroupsListView();
        }
    }
}