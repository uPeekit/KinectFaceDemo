using System;
using System.Collections.Generic;

namespace KinectDemo
{
    class DataConvertHandler
    {
        private MainWindow mainWindow;
        private DataConverter dataConverter = new DataConverter();

        public DataConvertHandler(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            mainWindow.SetConvertFilesList(new string[] { });
        }

        public void ChooseFiles()
        {
            string[] files = FilesHelper.PromptChooseFiles();
            dataConverter.FilesToConvert = files;
            mainWindow.SetConvertFilesList(files);
        }

        public void Convert()
        {
            Type strategyType = Type.GetType("KinectDemo." + mainWindow.GetChosenConvertOption());
            dataConverter.ConvertStrategy = (ConvertStrategy)Activator.CreateInstance(strategyType);
            if ((dataConverter.ConvertId = mainWindow.GetConvertId()).Length == 0)
                throw new Exception("set id");

            dataConverter.Convert(mainWindow.GetConvertParameters());
        }

        public string GetParametersDescription()
        {
            return dataConverter.ConvertStrategy.ParametersDescription;
        }

        public void SetConvertStrategy(string strategyName)
        {
            Type strategyType = Type.GetType("KinectDemo." + strategyName);
            dataConverter.ConvertStrategy = (ConvertStrategy)Activator.CreateInstance(strategyType);
        }
    }
}
