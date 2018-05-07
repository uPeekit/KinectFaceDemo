using System.Collections.Generic;
using System.Linq;

namespace KinectDemo
{
    public class NamedIndexedList
    {
        public string Name { get; set; }

        public Dictionary<int, double> IndexedValues { get; set; } = new Dictionary<int, double>();

        public NamedIndexedList(string fileName)
        {
            Name = fileName;
        }

        public void SetValuesAutoIndexed(List<double> values)
        {
            for(var i = 0; i < values.Count; ++i)
                IndexedValues.Add(i, values[i]);
        }

        public List<double> OnlyValues() => IndexedValues.Select(kvPair => kvPair.Value).ToList();

        public override string ToString() { return string.Format("{0}: {1} values", Name, IndexedValues.Count); }
    }
}