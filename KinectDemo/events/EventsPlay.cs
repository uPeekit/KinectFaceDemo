using KinectDemo.util;
using Microsoft.Kinect;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace KinectDemo
{
    public partial class MainWindow
    {
        private void FileDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                HandleChosenFile(files[0]);
            }
        }

        private void ChooseFile(object sender, RoutedEventArgs e)
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

        private void PlayFile(object sender, RoutedEventArgs e)
        {
            recordsPlayer.PlayFile();
        }

        private void StopPlayingFile(object sender, RoutedEventArgs e)
        {
            recordsPlayer.StopPlayingFile();
        }

        private void SliderXChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (recordsPlayer != null) recordsPlayer.recX = (float)e.NewValue;
        }

        private void SliderYChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (recordsPlayer != null) recordsPlayer.recY = (float)e.NewValue;
        }

        // setters

        public void SetPlayingFileName(string fileName)
        {
            PlayingFileName.Content = fileName;
        }
    }
}
