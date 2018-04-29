using System;
using System.Collections.Generic;
using System.Linq;

namespace KinectDemo
{
    internal class AvgDistanceAnalysisStrategy : AnalysisStrategy
    {
        private const int POINTS_COUNT = 1347;

        //private List<List<double>> pointsDeltaSums = new List<List<double>>();

        public void ConsumeFile(List<PointPositionsList> points)
        {
            //var pDeltas = CalculateDeltas(points);
            //var pDeltasSums = pDeltas.Select(nPoints => nPoints.Average()).ToList();

            //for (var i = 0; i < pDeltasSums.Count; ++i)
            //{
            //    if (pointsDeltaSums.Count <= i)
            //    {
            //        pointsDeltaSums.Add(new List<double>());
            //    }
            //    pointsDeltaSums[i].Add(pDeltasSums[i]);
            //}
        }

        //private List<List<double>> CalculateDeltas(List<PointPositionsList> points)
        //{
        //    List<List<double>> deltas = new List<List<double>>();
        //    PointPositionsList nPoints;
        //    for (var pointNumber = 0; pointNumber < points.Count; ++pointNumber)
        //    {
        //        nPoints = points[pointNumber];
        //        PointPositionsList.Position prevPoint = null;
        //        double delta;
        //        nPoints.ForEach(point =>
        //        {
        //            if (deltas.Count <= pointNumber)
        //                deltas.Add(new List<double>());

        //            if (prevPoint != null)
        //            {
        //                delta = GetDelta(point, prevPoint);
        //                deltas[pointNumber].Add(delta);
        //            }
        //            prevPoint = point;
        //        });
        //    }
        //    return deltas;
        //}
        
        //private double GetDelta(PointPositionsList.Position p1, PointPositionsList.Position p2)
        //{
        //    var xd = p1.X - p2.X;
        //    var yd = p1.Y - p2.Y;
        //    var zd = p1.Z - p2.Z;
        //    return Math.Sqrt( (xd*xd) + (yd*yd) + (zd*zd) );
        //}

        public List<double> GetResult()
        {
            //return pointsDeltaSums.Select(sums => sums.Average()).ToList();
            return null;
        }

    }
}