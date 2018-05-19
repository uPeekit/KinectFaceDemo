using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDemo
{
    class MaxDeltaPerFrame : UsingPointsDeltasConvertStrategy
    {
        public override string ResultFileName { get; set; } = "max_delta_per_frame";

        public override double DeltasToResult(List<double> deltas) => deltas.Max();
    }
}
