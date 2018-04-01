using System.Linq;
using System.IO;
using System;

namespace KinectDemo
{
    public class FileList
    {
        public string[] files { get; private set; }

        private FileList(string[] files) { this.files = files; }

        public static FileList Of(string[] files) { return new FileList(files); }

        public override string ToString()
        {
            return files.Length == 1 
                ? files[0].Split('\\').Last()
                : files.Aggregate((str1, str2) => string.Join(", ", new FileInfo(str1).Name, new FileInfo(str2).Name));
        }

        internal void Iterate(Action<string> action) { foreach (var file in files) action(file); }
    }

}