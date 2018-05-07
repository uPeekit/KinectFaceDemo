using System;
using System.Collections.Generic;
using System.Linq;

namespace KinectDemo
{
    class MostMovingPoints : ConvertStrategy
    {
        private List<NamedIndexedList> result;

        public override string ResultFileName { get; set; } = "most_moving_points";
        
        public override string ParametersDescription { get; set; } = "number of values";

        // use result of convertion by SumDistance strategy
        public override List<NamedIndexedList> ConsumeFiles(string parameters, params string[] files)
        {
            result = new List<NamedIndexedList>();
            int values_number = int.Parse(parameters);
            List<NamedIndexedList> parsedFiles = files.SelectMany(file => CsvHelper.ParseFileToFilesWithValues(file, true)).ToList();

            foreach (var fileWithValues in parsedFiles)
            {
                var i = 0;
                result.Add(new NamedIndexedList(fileWithValues.Name)
                {
                    IndexedValues = fileWithValues.IndexedValues.OrderByDescending(kvPair => kvPair.Value)
                                                  .Take(values_number)
                                                  .ToDictionary(kvPair => i++, kvPair => (double)kvPair.Key)
                });
            }
            return result;
        }

        public override bool DoIndexesMatter() => false;
    }
}
