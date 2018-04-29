using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        // getters

        public RecordMode GetRecordMode()
        {
            RecordMode mode = RadioForeign.IsChecked.Value ? Constants.MODE_FOREIGN
                                                           : (RadioNative.IsChecked.Value ? Constants.MODE_NATIVE
                                                                                          : null);
            return mode?.WithLanguage(language.Text)?.WithName(name.Text);
        }

        public bool IsNameSet()
        {
            return name.Text.Length != 0;
        }

        public bool IsLanguageSet()
        {
            return language.Text.Length != 0;
        }
    }
}
