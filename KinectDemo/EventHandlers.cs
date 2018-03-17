using System;
using System.Windows;

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

        private void SwitchRecord(object sender, RoutedEventArgs e)
        {
            if (_sensor == null || !_sensor.IsOpen)
            {
                return;
            }
            if (!_record)
            {
                //if (!_dataWriter.StartRecording())
                //{
                //    return;
                //}
                _record = true;
                _dataWriter = new DataWriter(this); // try this approach
                _dataWriter.StartRecording();
                recordLabel.Visibility = Visibility.Visible;
            }
            else
            {
                _record = false;
                _dataWriter.EndRecording();
                recordLabel.Visibility = Visibility.Hidden;
            }
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            _dataWriter.Reset();
        }

        public void SetBufferSize(int size, DataWriter writer)
        {
            ExecuteUsingDispatcher(() => buffer.Content = size.ToString(), writer);
        }
        public void SetExposedBufferSize(int size, DataWriter writer)
        {
            ExecuteUsingDispatcher(() => exposedBuffer.Content = size.ToString(), writer);
        }

        private void ExecuteUsingDispatcher(Action action, DataWriter writer)
        {
            if (writer != _dataWriter) return;
            Dispatcher.Invoke(action);
        }
    }
}
