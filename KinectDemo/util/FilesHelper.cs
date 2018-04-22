using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDemo.util
{
    class FilesHelper
    {
        public static string[] PromptChooseFiles()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".csv",
                InitialDirectory = Constants.DIR_BASE_OUTPUT,
                Multiselect = true
            };
            var result = dlg.ShowDialog();
            return result.HasValue && result.Value ? dlg.FileNames : null;
        }

        public static string PromptChooseFile()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".csv",
                InitialDirectory = @"C:\KinectData\",
            };
            var result = dlg.ShowDialog();
            return result.HasValue && result.Value ? dlg.FileName : null;
        }
    }
}
