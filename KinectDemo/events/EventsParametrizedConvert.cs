using System.Windows;
using System.Windows.Controls;

namespace KinectDemo
{
    partial class MainWindow
    {

        private void ParametrizedConvertType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            parametrizedConverterHandler.SetConvertStrategy(GetChosenParametrizedConvertOption());
        }
        
        private void ParametrizedConvert(object sender, RoutedEventArgs e)
        {
            ExecuteCatchingException(() =>
            {
                parametrizedConverterHandler.Convert();
                parametrizedConverterHandler = new ParametrizedConverterHandler(this);

                ShowPopupDone();
            });
        }

        //private void ChooseModelFiles(object sender, RoutedEventArgs e)
        //{
        //    trainModelHandler.ChooseFiles();
        //}

        // get

        public string GetChosenParametrizedConvertOption()
        {
            return ParametrizedConvertType.SelectedItem.ToString();
        }

        public string GetParametrizedConvertId()
        {
            return ParametrizedConvertId.Text;
        }

        // set

        //public void SetModelFiles(string[] files)
        //{
        //    ModelListView.ItemsSource = files;
        //}

    }
}
