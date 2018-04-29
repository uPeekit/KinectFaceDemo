using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDemo
{
    abstract class UsingPointsDeltasConvertStrategy : ConvertStrategy
    {
        public abstract string ResultFileName { get; set; }

        public List<double> ConsumeFile(string file)
        {
            List<PointPositionsList> pointsPositions = CsvHelper.ParseFileToPointsPositions(file);
            List<List<double>> pointsDeltas = CalculateDeltas(pointsPositions);

            return pointsDeltas.Select(DeltasToResultFunc()).ToList();
        }

        public abstract Func<List<double>, double> DeltasToResultFunc();

        private List<List<double>> CalculateDeltas(List<PointPositionsList> points)
        {
            List<List<double>> deltas = new List<List<double>>();
            PointPositionsList nPoints;
            for (var pointNumber = 0; pointNumber < points.Count; ++pointNumber)
            {
                nPoints = points[pointNumber];
                PointPositionsList.Position prevPoint = null;
                double delta;
                nPoints.ForEach(point =>
                {
                    if (deltas.Count <= pointNumber)
                        deltas.Add(new List<double>());

                    if (prevPoint != null)
                    {
                        delta = GetDelta(point, prevPoint);
                        deltas[pointNumber].Add(delta);
                    }
                    prevPoint = point;
                });
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

        public string GetLogSummary(string filePath, List<double> res)
        {
            return string.Format("{0} {4,-35} {1,-30} min: {2}, max: {3}", DateTime.Now, new FileInfo(filePath).Name, res.Min(), res.Max(), GetType().Name);
        }

        public bool DoIndexesMatter()
        {
            return true;
        }
    }
}
