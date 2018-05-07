using System;
using System.Collections.Generic;
using System.Linq;

namespace KinectDemo
{
    class AverageDistance : UsingPointsDeltasConvertStrategy
    {
        public override string ResultFileName { get; set; } = "distance_avg";

        public override double DeltasToResult(List<double> deltas) => deltas.Average();
    }
}
