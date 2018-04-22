using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        private static readonly string[] DIRS = new string[] { @"C:\KinectData\native", @"C:\KinectData\foreign" };

        static void Main(string[] args)
        {
            string line;
            long delta, timestamp, lastTimestamp, min, max;
            List<long> list;

            foreach (string dir in DIRS)
            {
                foreach(string file in Directory.GetFiles(dir)) { using (StreamReader fileStream = new StreamReader(file))
                    {
                        if (new FileInfo(file).Name.StartsWith("-"))
                            continue;

                        min = 10000;
                        max = 0;
                        lastTimestamp = -1;
                        list = new List<long>();

                        while ((line = fileStream.ReadLine()) != null)
                        {
                            timestamp = long.Parse(line.Substring(0, 14));
                            if (lastTimestamp > 0)
                            {
                                delta = timestamp - lastTimestamp;
                                list.Add(delta);
                                if (delta < min) min = delta;
                                if (delta > max) max = delta;
                            }
                            lastTimestamp = timestamp;
                        }
                        System.Diagnostics.Debug.WriteLine(string.Format("{0,-25} min: {1,2}, max {2,2}, avg: {3,3}", new FileInfo(file).Name, min, max, list.Average()));
                    }
                }
            }
        }
    }
}
