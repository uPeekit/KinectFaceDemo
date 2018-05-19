using System.Windows;
using System.Windows.Controls;

namespace KinectDemo
{
    public partial class MainWindow
    {
        private void Convert(object sender, RoutedEventArgs e)
        {
            //ExecuteCatchingException(() =>
            //{
                dataConvertHandler.Convert();

                ShowPopupDone();
            //});
        }

        private void ChooseConvertFiles(object sender, RoutedEventArgs e)
        {
            dataConvertHandler.ChooseFiles();
        }

        private void ConvertType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dataConvertHandler.SetConvertStrategy(GetChosenConvertOption());
            if ((ConvertParametersDescription.Content = dataConvertHandler.GetParametersDescription()) == null)
                SetVisibility(false, ConvertParametersDescription, ConvertParameters);
            else
                SetVisibility(true, ConvertParametersDescription, ConvertParameters);
        }

        private void SetVisibility(bool shohuldBeVisible, params UIElement[] elements)
        {
            foreach (var el in elements) { el.Visibility = shohuldBeVisible ? Visibility.Visible : Visibility.Hidden; }
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

        public string GetConvertId()
        {
            return ConvertId.Text;
        }

        public string GetConvertParameters()
        {
            return ConvertParameters.Text;
        }

    }
}
