using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDemo
{
    public interface ConvertStrategy
    {
        string ResultFileName { get; set; }

        List<double> ConsumeFile(string file);

        string GetLogSummary(string filePath, List<double> res);

        bool DoIndexesMatter();
    }
}
