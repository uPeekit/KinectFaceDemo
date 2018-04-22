using KinectDemo.data;
using KinectDemo.util;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

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
            dataAnalyst = new DataAnalysisHandler(this);
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

        public RecordMode GetRecordMode()
        {
            RecordMode mode = RadioForeign.IsChecked.Value ? Constants.MODE_FOREIGN 
                                                           : (RadioNative.IsChecked.Value ? Constants.MODE_NATIVE
                                                                                          : null);
            return mode?.WithLanguage(language.Text)?.WithName(name.Text);
        }

        // textbox

        public bool IsNameSet()
        {
            return name.Text.Length != 0;
        }

        public bool IsLanguageSet()
        {
            return language.Text.Length != 0;
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

        // play

        private void FileDrop(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                HandleChosenFile(files[0]);
            }
        }

        public void ChooseFile(object sender, RoutedEventArgs e)
        {
            HandleChosenFile(FilesHelper.PromptChooseFile());
        }

        private void HandleChosenFile(string file)
        {
            recordsPlayer.filePath = file;
            PlayingFileName.Content = file;
        }

        public void AddPlayerPoint(Ellipse ellipse)
        {
            playerCanvas.Children.Add(ellipse);
        }

        public void DisplayPoint(Ellipse point, DepthSpacePoint mappedCoords)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Canvas.SetLeft(point, mappedCoords.X);
                Canvas.SetTop(point, mappedCoords.Y);
            }));
        }

        public void SetPlayingFileName(string fileName)
        {
            PlayingFileName.Content = fileName;
        }

        public void PlayFile(object sender, RoutedEventArgs e)
        {
            recordsPlayer.PlayFile();
        }

        public void StopPlayingFile(object sender, RoutedEventArgs e)
        {
            recordsPlayer.StopPlayingFile();
        }
        
        public void SliderXChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(recordsPlayer != null) recordsPlayer.recX = (float)e.NewValue;
        }

        public void SliderYChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (recordsPlayer != null) recordsPlayer.recY = (float)e.NewValue;
        }

    }
}
