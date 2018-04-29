using System;
using System.Collections.Generic;
using System.Linq;

namespace KinectDemo
{
    class AverageDistanceConvertStrategy : UsingPointsDeltasConvertStrategy
    {
        public override string ResultFileName { get; set; } = Constants.RESULT_DISTANCE_AVG;

        public override Func<List<double>, double> DeltasToResultFunc()
        {
            return pDeltas => pDeltas.Average();
        }
    }
}
