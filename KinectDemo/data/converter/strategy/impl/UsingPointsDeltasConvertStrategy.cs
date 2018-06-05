using System;
using System.Collections.Generic;
using System.Linq;

namespace KinectDemo
{
    abstract class UsingPointsDeltasConvertStrategy : ConvertStrategy
    {
        public override string ParametersDescription { get; set; } = null;

        public override List<NamedIndexedList> ConsumeFiles(string parameters, params string[] files)
        {
            List<List<PointPositionsList>> filesPointsPositions = files.Select(file => CsvHelper.ParseFileToPointsPositions(file)).ToList();
            // here rescale to be relative
            filesPointsPositions = RescaleRelatively(filesPointsPositions);

            List<List<List<double>>> filesPointsDeltas = filesPointsPositions.Select(filePointPositions => CalculateDeltas(filePointPositions)).ToList();
            List<List<double>> filesResults = filesPointsDeltas.Select(pointsDeltas => pointsDeltas.Select(DeltasToResult).ToList()).ToList();

            return files.Select(file => new NamedIndexedList(file))
                        .Select((fileWithValues, fileIndex) =>
                        {
                            fileWithValues.SetValuesAutoIndexed(filesResults[fileIndex]);
                            return fileWithValues;
                        }).ToList();
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

        public abstract double DeltasToResult(List<double> deltas);

        protected List<List<double>> CalculateDeltas(List<PointPositionsList> points)
        {
            List<List<double>> deltas = new List<List<double>>();
            PointPositionsList nPoints;
            for (var pointNumber = 0; pointNumber < points.Count; ++pointNumber)
            {
                nPoints = points[pointNumber];
                PointPositionsList.Position prevPoint = null;
                double delta;
                nPoints.ForEach(point =>
                {
                    if (deltas.Count <= pointNumber)
                        deltas.Add(new List<double>());

                    if (prevPoint != null)
                    {
                        delta = GetDelta(point, prevPoint);
                        deltas[pointNumber].Add(delta);
                    }
                    prevPoint = point;
                });
            }
            return deltas;
        }

        protected double GetDelta(PointPositionsList.Position p1, PointPositionsList.Position p2)
        {
            var xd = p1.X - p2.X;
            var yd = p1.Y - p2.Y;
            var zd = p1.Z - p2.Z;
            return Math.Sqrt((xd * xd) + (yd * yd) + (zd * zd));
        }

        public override bool DoIndexesMatter() => true;
    }
}
