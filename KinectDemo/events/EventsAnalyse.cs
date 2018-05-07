using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KinectDemo
{
    public partial class MainWindow
    {
        private void Analyse(object sender, RoutedEventArgs e)
        {
            ExecuteCatchingException(() =>             
            {
                dataAnalysisHandler.Analyse();

                SetAnalyseFiles(new string[] { });
                dataAnalysisHandler.ResetFile();

                ShowPopupDone();
            });
        }

        private void ChooseAnalyseFile(object sender, RoutedEventArgs e)
        {
            dataAnalysisHandler.ChooseFile();
        }

        // getters
        
        public string GetAnalyseId()
        {
            return AnalyseId.Text;
        }

        // setters

        public void SetAnalyseFiles(string[] files)
        {
            AnalyseFileListView.ItemsSource = files;
        }

    }
}
