using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KinectDemo
{
    public partial class MainWindow
    {
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

        public void RefreshGroupsListView()
        {
            GroupsListView.Items.Refresh();
        }

        // getters

        public string GetChosenAnalysisOption()
        {
            return AnalysisType.SelectedItem.ToString();
        }

        public List<string[]> GetGroupsList()
        {
            return (List<string[]>)GroupsListView.ItemsSource;
        }

        // setters

        public void SetGroupsList(List<FileList> groupsList)
        {
            GroupsListView.ItemsSource = groupsList;
        }
    }
}
