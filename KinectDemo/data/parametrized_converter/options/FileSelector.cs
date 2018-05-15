using System;
using System.Windows;
using System.Windows.Controls;

namespace KinectDemo
{
    class FileSelector : Parameter<FileSelector>
    {
        public override string Name { get; set; }

        public override Action<FileSelector> ElementProcessor { get; set; }

        public Button Button { get; set; }
        public Label Description { get; set; }
        public string[] Files { get; set; }
        ListView FilesView { get; set; }

        public FileSelector(string name, Action<FileSelector> processElement) : base(name)
        {
            ElementProcessor = processElement;
        }

        public override void Clear()
        {
            FilesView.ItemsSource = Files = new string[] { };
        }

        public override void CreateElement(UIElementCollection collectionToAdd)
        {
            Description = new Label() { Content = this.Name, Width = 200, HorizontalAlignment = HorizontalAlignment.Left };
            FilesView = new ListView() { Margin = new Thickness(5, 5, 5, 10), Width = 350, Height = 70, HorizontalAlignment = HorizontalAlignment.Left };
            Button = new Button()
            {
                Content = "Choose",
                Margin = new Thickness(5), Width = 70, Height = 30,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            Button.Click += (object sender, RoutedEventArgs e) => {
                FilesView.ItemsSource = Files = FilesHelper.PromptChooseFiles();
            };
            collectionToAdd.Add(Description);
            collectionToAdd.Add(Button);
            collectionToAdd.Add(FilesView);
        }
    }
}
