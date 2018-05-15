using System.Collections.Generic;
using System.Linq;

namespace KinectDemo
{
    class CommonValues : NoIndexesConvertStrategy
    {
        public override string ResultFileName { get; set; } = "common_values";

        public override string ParametersDescription { get; set; } = null;

        private NamedIndexedList result;

        public override List<NamedIndexedList> ConsumeFiles(string parameters, params string[] files)
        {
            List<NamedIndexedList> filesWithValues = files.SelectMany(file => CsvHelper.ParseFileToNamedIndexedLists(file, false)).ToList();
            var i = 0;
            result = new NamedIndexedList("combined")
            {
                IndexedValues = filesWithValues.Select(fv => fv.OnlyValues())
                                                  .Aggregate((list1, list2) => list1.Intersect(list2).ToList())
                                                  .ToDictionary(val => i++)
            };
            return new List<NamedIndexedList> { result };
        }
    }
}
