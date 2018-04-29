using System.Collections.Generic;

namespace KinectDemo
{
    public interface AnalysisStrategy
    {
        void ConsumeFile(List<PointPositionsList> points);

        List<double> GetResult();
    }

}