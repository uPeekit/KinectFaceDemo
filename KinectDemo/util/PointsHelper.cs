using Microsoft.Kinect.Face;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDemo
{
    class PointsHelper
    {
        public static Dictionary<string, double> ExtractNamedValues(List<double> list)
        {
            var specialIter = Enum.GetValues(typeof(HighDetailFacePoints)).Cast<HighDetailFacePoints>();
            return specialIter.ToDictionary(pointDef => pointDef.ToString(), pointDef => list[(int)pointDef]);
        }
    }
}
