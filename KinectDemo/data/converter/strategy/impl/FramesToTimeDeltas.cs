using System.Collections.Generic;

namespace KinectDemo
{
    class FramesToTimeDeltas : NoIndexesConvertStrategy
    {
        private long delta, timestamp, lastTimestamp;
        private List<double> CurrentResult;

        public override string ResultFileName { get; set; } = "time_deltas";

        public override string ParametersDescription { get; set; } = null;

        public FramesToTimeDeltas()
        {
            ResetFields();
        }

        public override List<NamedIndexedList> ConsumeFiles(string parameters, params string[] files)
        {
            List<NamedIndexedList> result = new List<NamedIndexedList>();
            NamedIndexedList fileWithValues;
            foreach (var file in files)
            {
                fileWithValues = new NamedIndexedList(file);
                CurrentResult = new List<double>();
                CsvHelper.ReadCsvByLines(file, AddDataLine);
                fileWithValues.SetValuesAutoIndexed(CurrentResult);
            }
            return result;
        }

        private void AddDataLine(int lineNumber, string line)
        {
            timestamp = long.Parse(line.Substring(0, 14));
            if (lastTimestamp > 0)
            {
                delta = timestamp - lastTimestamp;
                CurrentResult.Add(delta);
            }
            lastTimestamp = timestamp;
        }

        private void ResetFields()
        {
            lastTimestamp = -1;
        }
    }
}
