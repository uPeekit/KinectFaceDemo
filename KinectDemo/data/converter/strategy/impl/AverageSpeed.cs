using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDemo
{
    class AverageSpeed : UsingPointsDeltasConvertStrategy
    {
        public override string ResultFileName { get; set; } = "average_speed";

        public override string ParametersDescription { get; set; } = null;

        private List<List<long>> timeDeltas;
        private int currentFileNumber;

        public override List<NamedIndexedList> ConsumeFiles(string parameters, params string[] files)
        {
            List<List<PointPositionsList>> filesPointsPositions = files.Select(file => CsvHelper.ParseFileToPointsPositions(file)).ToList();
            List<List<List<double>>> filesPointsDeltas = filesPointsPositions.Select(filePointPositions => CalculateDeltas(filePointPositions)).ToList();
            timeDeltas = filesPointsPositions.Select(filePointPositions => CalculateTimeDeltas(filePointPositions)).ToList();
            List <List<double>> filesResults = filesPointsDeltas.Select((pointsDeltas, i) =>
                {
                    currentFileNumber = i;
                    return pointsDeltas.Select(DeltasToResult).ToList();
                }).ToList();

            return files.Select(file => new NamedIndexedList(file))
                        .Select((fileWithValues, fileIndex) =>
                        {
                            fileWithValues.SetValuesAutoIndexed(filesResults[fileIndex]);
                            return fileWithValues;
                        }).ToList();
        }

        private List<long> CalculateTimeDeltas(List<PointPositionsList> filePointPositions)
        {
            List<long> res = new List<long>();
            long prevTimestamp = -1;
            filePointPositions[0].timestamps.ForEach(ts =>
            {
                if(prevTimestamp > 0)
                {
                    res.Add(ts - prevTimestamp);
                }
                prevTimestamp = ts;
            });
            return res;
        }

        public override double DeltasToResult(List<double> deltas)
        {
            List<double> distToTime = new List<double>();
            for(var i = 0; i < deltas.Count; ++i)
            {
                distToTime.Add(deltas[i] / timeDeltas[currentFileNumber][i]);
            }
            return distToTime.Average();
        }

        public override bool DoIndexesMatter() => true;
    }
}
