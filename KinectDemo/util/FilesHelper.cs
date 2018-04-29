using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDemo
{
    class FilesHelper
    {
        public static string GetIncreasedVersionOfFile(string directory, string fileName)
        {
            string[] existingNumbers = Directory.GetFiles(directory).Where(file => new FileInfo(file).Name.StartsWith(fileName))
                                                                    .Select(file => file.Remove(0, directory.Length + fileName.Length).Split('.').First())
                                                                    .ToArray();
            if (existingNumbers.Length == 0) return fileName;

            int maxNumber = Array.ConvertAll(existingNumbers,
                str => int.TryParse(str, out int parsed) ? parsed : 0).Max();

            return fileName + (maxNumber + 1);
        }

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

        public static void WriteLogLine(string logPath, string line)
        {
            using (var stream = new StreamWriter(File.Open(logPath, FileMode.Append)))
            {
                stream.WriteLine(line);
            }
        }
    }
}
