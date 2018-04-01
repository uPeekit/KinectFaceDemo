using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace KinectDemo
{
    public partial class MainWindow
    {
        private void SwitchSensor(object sender, RoutedEventArgs e)
        {
            if (_sensor == null)
            {
                return;
            }
            if (_sensor.IsOpen)
            {
                _sensor.Close();
                canvas.Children.Clear();
                _points.Clear();
            }
            else
            {
                _sensor.Open();
            }
        }

        public bool IsSensorOpen()
        {
            return _sensor != null && _sensor.IsOpen;
        }

        private void SwitchRecord(object sender, RoutedEventArgs e)
        {
            if (!record)
            {
                dataWriter = new DataWriter(this);
                try { dataWriter.StartRecording(); }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Oops");
                    return;
                }
                record = true;
                recordLabel.Visibility = Visibility.Visible;
            }
            else
            {
                record = false;
                dataWriter.EndRecording();
                recordLabel.Visibility = Visibility.Hidden;
            }
        }

        private void Analyse(object sender, RoutedEventArgs e)
        {
            dataAnalyst.Analyse();
            dataAnalyst = new DataAnalyst(this);
        }

        private void AddGroup(object sender, RoutedEventArgs e)
        {
            dataAnalyst.AddGroup();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var value = ((ListViewItem)sender).Content as FileList;
            dataAnalyst.RemoveGroup(value);
        }

        private void Button_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.Source.GetType().Equals(typeof(TextBox)) && e.Key.Equals(Key.R))
                SwitchRecord(sender, null);
        }

        // setters

        public void SetBufferSize(int size, DataWriter writer)
        {
            if (writer != dataWriter) return;
            Dispatcher.Invoke(() => buffer.Content = size.ToString());
        }
        
        public void SetFileName(string name)
        {
            Dispatcher.Invoke(() => fileName.Content = name);
        }

        // combobox

        public string GetChosenAnalysisOption()
        {
            return AnalysisType.SelectedItem.ToString();
        }

        // radio

        public enum Mode { FOREIGN, NATIVE, NONE }

        public Mode GetMode()
        {
            return RadioForeign.IsChecked.Value ? Mode.FOREIGN 
                                                : (RadioNative.IsChecked.Value ? Mode.NATIVE 
                                                                               : Mode.NONE);
        }

        // textbox

        public string GetName()
        {
            return name.Text;
        }

        // groups list view

        public List<string[]> GetGroupsList()
        {
            return (List<string[]>)GroupsListView.ItemsSource;
        }

        public void SetGroupsList(List<FileList> groupsList)
        {
            GroupsListView.ItemsSource = groupsList;
        }

        public void RefreshListView()
        {
            GroupsListView.Items.Refresh();
        }

    }
}
