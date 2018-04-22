using System.Collections.Generic;
using KinectDemo.data;

namespace KinectDemo
{
    public interface AnalysisStrategy
    {
        void AddFilePoints(List<PointPositionsList> points);

        List<double> GetResult();
    }

}