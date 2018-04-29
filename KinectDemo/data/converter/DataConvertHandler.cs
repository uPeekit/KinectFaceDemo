using KinectDemo.util;
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
            dataConverter.Files = files;
            mainWindow.SetConvertFilesList(files);
        }

        public void Convert()
        {
            Type strategyType = Type.GetType("KinectDemo." + mainWindow.GetChosenConvertOption());
            dataConverter.ConvertStrategy = (ConvertStrategy)Activator.CreateInstance(strategyType);

            dataConverter.Convert();
        }
    }
}
