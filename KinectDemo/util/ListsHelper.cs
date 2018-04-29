using System;
using System.Collections.Generic;
using System.Linq;

namespace KinectDemo
{
    class ListsHelper
    {
        public static List<List<string>> ValuesColumnsToDataLines(List<List<double>> valuesColumns, bool indexesAsFirstColumn)
        {
            List<List<string>> result = new List<List<string>>();
            List<string> currentDataLine = null;
            int linesCount = valuesColumns[0].Count;

            for (var resultLineIndex = 0; resultLineIndex < linesCount; ++resultLineIndex)
            {
                if (result.Count <= resultLineIndex) result.Add(currentDataLine = new List<string>());
                for (var currentColumnIndex = 0; currentColumnIndex < valuesColumns.Count; ++currentColumnIndex)
                {
                    if (currentDataLine.Count == 0 && indexesAsFirstColumn)
                    {
                        currentDataLine.Add(resultLineIndex.ToString());
                    }
                    currentDataLine.Add(valuesColumns[currentColumnIndex][resultLineIndex].ToString());
                }
            }
            return result;
        }

        public static List<List<string>> ValuesListToDataLines(List<double> values, bool indexesAsFirstColumn)
        {
            List<string> list;
            return values.Select((val, i) => 
            {
                list = new List<string>();
                if (indexesAsFirstColumn) list.Add(i.ToString());
                list.Add(val.ToString());
                return list;
            }).ToList();
        }

    }
}
