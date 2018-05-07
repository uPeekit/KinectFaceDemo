using System;
using System.Collections.Generic;
using System.Linq;

namespace KinectDemo
{
    public abstract class ConvertStrategy
    {
        public abstract string ResultFileName { get; set; }

        public abstract string ParametersDescription { get; set; }

        public abstract List<NamedIndexedList> ConsumeFiles(string parameters, params string[] files);

        public abstract bool DoIndexesMatter();

        public string GetLogSummary(List<NamedIndexedList> result)
        {
            if (result.Count == 1 && (result[0].IndexedValues == null || result[0].IndexedValues.Count == 0))
                return string.Format("{0}\t {1,35} noresult", DateTime.Now, GetType().Name);

            return result.Select(fileRes => string.Format("{0}\t {1,35} {2,35} min: {3}, max {4}, avg: {5}",
                DateTime.Now, GetType().Name, fileRes.Name, fileRes.OnlyValues().Min(), fileRes.OnlyValues().Max(), fileRes.OnlyValues().Average()))
                         .Aggregate((str1, str2) => string.Join("\n", str1, str2));
        }
    }
}
