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

        public void AddFilePoints(List<List<Tuple<double, double, double>>> points)
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

        private List<List<double>> CalculateDeltas(List<List<Tuple<double, double, double>>> points)
        {
            List<List<double>> deltas = new List<List<double>>();
            //var scales = points[0].Select(tuple => tuple.Item1).ToList();

            for (var pointNumber = 1; pointNumber < points.Count; ++pointNumber)
            {
                Tuple<double, double, double> point, prevPoint = null;
                var nPoints = points[pointNumber];
                double delta;
                //double scale;
                for (var frameNumber = 0; frameNumber < nPoints.Count; ++frameNumber)
                {
                    //scale = scales[frameNumber];
                    point = nPoints[frameNumber];

                    if (deltas.Count < pointNumber)
                        deltas.Add(new List<double>());
                    if (prevPoint != null)
                    {
                        delta = GetDelta(point, prevPoint);
                        //deltas[pointNumber - 1].Add(delta * 100 / scale);
                        deltas[pointNumber - 1].Add(100);
                    }
                    prevPoint = point;
                }
            }
            return deltas;
        }
        
        private double GetDelta(Tuple<double, double, double> p1, Tuple<double, double, double> p2)
        {
            var xd = p1.Item1 - p2.Item1;
            var yd = p1.Item2 - p2.Item2;
            var zd = p1.Item3 - p2.Item3;
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