using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDemo
{
    class Rescale : ConvertStrategy
    {
        public override string ResultFileName { get; set; } = "rescaled";
        public override string ParametersDescription { get; set; }

        public override List<NamedIndexedList> ConsumeFiles(string parameters, params string[] files)
        {
            List<List<PointPositionsList>> filesPointsPositions = files.Select(file => CsvHelper.ParseFileToPointsPositions(file)).ToList();
            
            var result = new List<List<string>>();
            var allPoints = RescaleRelatively(filesPointsPositions)[0];
            List<PointPositionsList.Position> pPositions;
            PointPositionsList.Position pos;
            for (var pointN = 0; pointN < allPoints.Count; ++pointN)
            {
                pPositions = allPoints[pointN].positions;
                for (var posN = 0; posN < pPositions.Count; ++posN)
                {
                    pos = pPositions[posN];
                    if(result.Count <= posN) result.Add(new List<string> { (1 + 100*posN).ToString() });
                    result[posN].Add(string.Format("{0} {1} {2} {3}", pointN, pos.X, pos.Y, pos.Z));
                }
            }
            CsvHelper.WriteCsv(files[0].Insert(files[0].Length-4,"_rescaled"), null, result);
            // dirty hack to avoid further file creation
            throw new Exception("rescaling done");
        }

        private List<List<PointPositionsList>> RescaleRelatively(List<List<PointPositionsList>> filesPointsPositions)
        {
            // 24 is nose top
            List<PointPositionsList> filePointsPositions;
            List<PointPositionsList.Position> noseTopPositions, pointPositions;
            PointPositionsList.Position pnose, pos;
            for (var fileN = 0; fileN < filesPointsPositions.Count; ++fileN)
            {
                filePointsPositions = filesPointsPositions[fileN];
                noseTopPositions = filePointsPositions.Find(pointPos => pointPos.PointNumber == 24).positions
                                                      .Select(p => PointPositionsList.Position.Of(p.X, p.Y, p.Z)).ToList();
                for (var pointN = 0; pointN < filePointsPositions.Count; ++pointN)
                {
                    pointPositions = filePointsPositions[pointN].positions;
                    for (var posN = 0; posN < pointPositions.Count; ++posN)
                    {
                        pnose = noseTopPositions[posN];
                        pos = pointPositions[posN];
                        filesPointsPositions[fileN][pointN].positions[posN] = PointPositionsList.Position.Of(pnose.X - pos.X, pnose.Y - pos.Y, pnose.Z - pos.Z);
                    }
                }
            }
            return filesPointsPositions;
        }

        public override bool DoIndexesMatter()
        {
            throw new NotImplementedException();
        }
    }
}
