using System;
using System.Collections.Generic;

namespace KinectDemo.data
{
    public class PointList
    {
        public class Point
        {
            public double X;
            public double Y;
            public double Z;

            private Point(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public static Point Of(double x, double y, double z) { return new Point(x, y, z); }
        }

        private List<Point> points = new List<Point>();

        public void Add(Point p) { points.Add(p); }

        public int Size() { return points.Count; }

        public void Iterate(Action<Point> action)
        {
            points.ForEach(action);
        }
    }
}
