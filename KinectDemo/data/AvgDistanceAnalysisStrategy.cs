using KinectDemo.data;
using Microsoft.Kinect.Face;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KinectDemo
{
    internal class AvgDistanceAnalysisStrategy : AnalysisStrategy
    {
        private const int POINTS_COUNT = 1347;

        private string directory;
        private List<List<double>> pointsDeltaSums = new List<List<double>>();

        public AvgDistanceAnalysisStrategy(string directory)
        {
            this.directory = directory;
        }

        public void AddFilePoints(List<PointList> points)
        {
            var pDeltas = CalculateDeltas(points);
            var pDeltasSums = pDeltas.Select(nPoints => nPoints.Sum()).ToList();

            for (var i = 0; i < pDeltasSums.Count; ++i)
            {
                if (pointsDeltaSums.Count <= i)
                {
                    pointsDeltaSums.Add(new List<double>());
                }
                pointsDeltaSums[i].Add(pDeltasSums[i]);
            }
        }

        private List<List<double>> CalculateDeltas(List<PointList> points)
        {
            List<List<double>> deltas = new List<List<double>>();
            PointList nPoints;
            for (var pointNumber = 0; pointNumber < points.Count; ++pointNumber)
            {
                nPoints = points[pointNumber];
                PointList.Point prevPoint = null;
                double delta;
                nPoints.Iterate(point =>
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
        
        private double GetDelta(PointList.Point p1, PointList.Point p2)
        {
            var xd = p1.X - p2.X;
            var yd = p1.Y - p2.Y;
            var zd = p1.Z - p2.Z;
            return Math.Sqrt( (xd*xd) + (yd*yd) + (zd*zd) );
        }

        public Dictionary<string, double> Process()
        {
            var deltaSumsAvg = pointsDeltaSums.Select(sums => sums.Average()).ToList();
            var specialIter = Enum.GetValues(typeof(HighDetailFacePoints)).Cast<HighDetailFacePoints>();
            var result = SpecialPointsValuesFrom(specialIter, deltaSumsAvg);
            pointsDeltaSums.Clear();
            return result;
        }

        private Dictionary<string, double> SpecialPointsValuesFrom(IEnumerable<HighDetailFacePoints> specialIter, List<double> listFrom)
        {
            return specialIter.ToDictionary(pointDef => pointDef.ToString(), pointDef => listFrom[(int)pointDef]);
        }
    }
}