using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDemo
{
    class DummyAnalyse : IAnalysisStrategy
    {
        public string ResultFileName { get; set; }
        public string Parameters { get; set; }
        public string ParametersDescription { get; set; } = "dummy";

        public List<NamedIndexedList> ConsumeFilesConverted(string parameters, params string[] file)
        {
            throw new NotImplementedException();
        }

        public Func<KeyValuePair<int, double>, string> GetValueToWriteExtractor()
        {
            throw new NotImplementedException();
        }
    }
}
