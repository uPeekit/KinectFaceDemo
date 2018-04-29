using System;
using System.Collections.Generic;
using System.Windows;

namespace KinectDemo
{
    public partial class MainWindow
    {
        private void Convert(object sender, RoutedEventArgs e)
        {
            try
            {
                dataConverter.Convert();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Oops");
                return;
            }
            dataConverter = new DataConvertHandler(this);
        }

        private void ChooseConvertFiles(object sender, RoutedEventArgs e)
        {
            dataConverter.ChooseFiles();
        }

        // setters

        public void SetConvertFilesList(string[] filesList)
        {
            ConvertFilesListView.ItemsSource = filesList;
        }

        // getters

        public string GetChosenConvertOption()
        {
            return ConvertType.SelectedItem.ToString();
        }
    }
}
