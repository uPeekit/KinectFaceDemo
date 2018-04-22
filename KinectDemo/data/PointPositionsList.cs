using System;
using System.Collections.Generic;

namespace KinectDemo.data
{
    public class PointPositionsList
    {
        public class Position
        {
            public double X, Y, Z;

            private Position(double x, double y, double z) { X = x; Y = y; Z = z; }

            public static Position Of(double x, double y, double z) { return new Position(x, y, z); }

            public override string ToString() { return string.Format("{0} {1} {2}", X, Y, Z); }
        }

        public int PointNumber { get; private set; }

        private PointPositionsList(int pointNumber) { PointNumber = pointNumber; }

        public static PointPositionsList For(int n) { return new PointPositionsList(n); }

        private List<Position> positions = new List<Position>();

        public void Add(Position p) { positions.Add(p); }

        public int Size() { return positions.Count; }

        public void ForEach(Action<Position> action) { positions.ForEach(action); }
    }
}
