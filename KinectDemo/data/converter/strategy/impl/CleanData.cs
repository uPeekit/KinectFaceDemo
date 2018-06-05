using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDemo
{
    class CleanData : ConvertStrategy
    {
        public override string ResultFileName { get; set; }
        public override string ParametersDescription { get; set; }

        public override List<NamedIndexedList> ConsumeFiles(string parameters, params string[] files)
        {
            foreach (var file in files)
            {
                List<List<string>> res = new List<List<string>>();
                bool first = true;
                CsvHelper.ReadCsvByTokens(file, null, (lineN, token) =>
                {
                    if (res.Count <= lineN) first = true;
                    if(first)
                    {
                        res.Add(new List<string>());
                        first = false;
                    }
                    else
                    {
                        res[lineN].Add(token.Substring(token.IndexOf(' ')+1));
                    }
                });
                CsvHelper.WriteCsv(file.Insert(file.Length - 4, "_new"), null, res);
            }
            // dirty hack to avoid further file creation
            throw new Exception("clean done");
        }

        public override bool DoIndexesMatter()
        {
            throw new NotImplementedException();
        }
    }
}
