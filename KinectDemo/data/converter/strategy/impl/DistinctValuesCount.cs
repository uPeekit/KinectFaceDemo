using System;
using System.Collections.Generic;
using System.Linq;

namespace KinectDemo
{
    class DistinctValuesCount : ConvertStrategy
    {
        public override string ResultFileName { get; set; } = "distinct_values_count";

        public override string ParametersDescription { get; set; } = null;

        private Dictionary<int, int> valuesCounts = new Dictionary<int, int>();

        public override List<NamedIndexedList> ConsumeFiles(string parameters, params string[] files)
        {
            List<NamedIndexedList> filesWithValues = files.SelectMany(file => CsvHelper.ParseFileToNamedIndexedLists(file, false)).ToList();
            foreach (var fileWithValues in filesWithValues)
            {
                foreach (int value in fileWithValues.OnlyValues())
                {
                    valuesCounts.TryGetValue(value, out int count);
                    valuesCounts[value] = ++count;
                }
            }
            return new List<NamedIndexedList> {
                new NamedIndexedList("count")
                {
                    IndexedValues = valuesCounts.OrderByDescending(kvPair => kvPair.Value)
                                                .ToDictionary(kvPair => kvPair.Key, kvPair => (double)kvPair.Value)
                }
            };
        }

        public override bool DoIndexesMatter() => true;
    }
}
