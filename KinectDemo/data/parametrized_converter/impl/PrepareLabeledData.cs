using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDemo
{
    class PrepareLabeledData : IParametrizedConvertStrategy
    {
        public List<Parameter> Parameters { get; set; }

        public string ResultFileName => "prepared_data";

        private List<int> Indexes;
        private List<List<string>> foreignSums;
        private List<List<string>> nativeSums;

        public PrepareLabeledData()
        {
            Parameters = new List<Parameter>
            {
                new FileSelector("point index counts", selector => {
                    var parsed = selector.Files.SelectMany(file => CsvHelper.ParseFileToNamedIndexedLists(file, true)).ToList();
                    Indexes = parsed[0].IndexedValues.Select(kvPair => kvPair.Key).Take(200).ToList();
                }),
                new FileSelector("delta sums foreign", selector => {
                    var foreignLists = selector.Files.SelectMany(file => CsvHelper.ParseFileToNamedIndexedLists(file, true)).ToList();
                    foreignSums = foreignLists.Select(list => list.IndexedValues.Where(kvPair => Indexes.Contains(kvPair.Key)))
                                              .Select(dict => dict.Select(kvPair => kvPair.Value.ToString()).ToList())
                                              .ToList();
                }),
                new FileSelector("delta sums native", selector => {
                    var nativeLists = selector.Files.SelectMany(file => CsvHelper.ParseFileToNamedIndexedLists(file, true)).ToList();
                    nativeSums = nativeLists.Select(list => list.IndexedValues.Where(kvPair => Indexes.Contains(kvPair.Key)))
                                            .Select(dict => dict.Select(kvPair => kvPair.Value.ToString()).ToList())
                                            .ToList();
                })
            };
        }

        public List<List<string>> GetResultDataWithHeaders()
        {
            var headers = Indexes.OrderBy(i => i).Select(i => i.ToString()).ToList();
            headers.Add("label");

            foreignSums.ForEach(sumList => sumList.Add("foreign"));
            nativeSums.ForEach(sumList => sumList.Add("native"));

            var data = new List<List<string>>();
            data.Add(headers);
            foreignSums.ForEach(data.Add);
            nativeSums.ForEach(data.Add);

            return data;
        }

    }
}
