using System;
using System.Collections.Generic;
using System.Linq;

namespace KinectDemo
{
    class SumDistance : UsingPointsDeltasConvertStrategy
    {
        public override string ResultFileName { get; set; } = "distance_sum";

        public override double DeltasToResult(List<double> deltas) => deltas.Sum();
    }
}
