using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDemo
{
    class MaxDeltaWithStartPosition : ConvertStrategy
    {
        public override string ResultFileName { get; set; } = "max_delta_with_start_position";

        public override string ParametersDescription { get; set; } = null;

        public override List<NamedIndexedList> ConsumeFiles(string parameters, params string[] files)
        {
            List<List<PointPositionsList>> filesPointsPositions = files.Select(file => CsvHelper.ParseFileToPointsPositions(file)).ToList();
            List<List<double>> filesResults = filesPointsPositions.Select(filePointPositions => CalculateDeltas(filePointPositions)).ToList();

            return files.Select(file => new NamedIndexedList(file))
                        .Select((fileWithValues, fileIndex) =>
                        {
                            fileWithValues.SetValuesAutoIndexed(filesResults[fileIndex]);
                            return fileWithValues;
                        }).ToList();
        }

        private List<double> CalculateDeltas(List<PointPositionsList> points)
        {
            List<double> deltas = new List<double>();
            double currentDelta, currentMaxDelta;
            PointPositionsList nPoints;
            PointPositionsList.Position currentPosition, startPosition;
            for (var pointNumber = 0; pointNumber < points.Count; ++pointNumber)
            {
                currentMaxDelta = 0;
                nPoints = points[pointNumber];
                if (nPoints.positions.Count == 0) break;

                startPosition = nPoints.positions[0];
                for (var posNumber = 1; posNumber < nPoints.positions.Count; ++posNumber)
                {
                    currentPosition = nPoints.positions[posNumber];
                    currentDelta = GetDelta(currentPosition, startPosition);

                    if (currentDelta > currentMaxDelta) currentMaxDelta = currentDelta;
                }
                deltas.Add(currentMaxDelta);
            }
            return deltas;
        }

        private double GetDelta(PointPositionsList.Position p1, PointPositionsList.Position p2)
        {
            var xd = p1.X - p2.X;
            var yd = p1.Y - p2.Y;
            var zd = p1.Z - p2.Z;
            return Math.Sqrt((xd * xd) + (yd * yd) + (zd * zd));
        }

        public override bool DoIndexesMatter() => true;
    }
}
