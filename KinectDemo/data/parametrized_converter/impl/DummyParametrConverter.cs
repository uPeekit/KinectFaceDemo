using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KinectDemo
{
    class DummyParametrConverter : IParametrizedConvertStrategy
    {
        public List<Parameter> Parameters { get; set; } = new List<Parameter>
        {
            new FileSelector("dummy files", param => {
                FileSelector selector = (FileSelector)param;
                MessageBox.Show(string.Format("{0}\n{1}", selector.Name, string.Join(",", selector.Files)), "selector");
            })
        };

        public string ResultFileName => "dummy";

        public List<List<string>> GetResultDataWithHeaders()
        {
            return new List<List<string>> { new List<string> { } };
        }
    }
}
