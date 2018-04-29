using System;
using System.Collections.Generic;
using System.Linq;

namespace KinectDemo
{
    class SumDistanceConvertStrategy : UsingPointsDeltasConvertStrategy
    {
        public override string ResultFileName { get; set; } = Constants.RESULT_DISTANCE_SUM;

        public override Func<List<double>, double> DeltasToResultFunc()
        {
            return pDeltas => pDeltas.Sum();
        }
    }
}
