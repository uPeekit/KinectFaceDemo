using System;
using System.Collections.Generic;

namespace KinectDemo
{
    public interface IAnalysisStrategy
    {
        string ResultFileName { get; set; }
        
        List<NamedIndexedList> ConsumeFilesConverted(string parameters, params string[] files);

        // pair <index, value>
        Func<KeyValuePair<int, double>, string> GetValueToWriteExtractor();
    }

}