using BLL.DTO;
using BLL.Enums;
using BLL.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using BLL.Exceptions;

namespace BLL
{
    public class MainCalculation
    {
        private readonly string _fileConnectionString;
        private readonly InformationModelDto _informationModel;
        private AdditionalTablesModelDto _additionalTablesModelDto;
        public CalculationResult CalculationResult { get; private set; }


        public MainCalculation(InformationModelDto informationModel, AdditionalTablesModelDto additionalTablesModelDto, string fileConnectionString)
        {
            _informationModel = informationModel;
            _additionalTablesModelDto = additionalTablesModelDto;
            _fileConnectionString = fileConnectionString;
        }

        public void Calculate()
        {
            SortedDictionary<double, List<Point>> allPoints = new SortedDictionary<double, List<Point>>();
            SortedDictionary<double, List<Point>> convexHullPoints = new SortedDictionary<double, List<Point>>();
            List<double> distancesZ = new List<double>();
            List<double> squares = new List<double>();
            int numberOfLastZCoordinate = 0;
            Volume volume = new Volume();
            double defformation = 1 + ((15 - _informationModel.Temperature) * _informationModel.LinearTempCoeff);
            List<double> keys = new List<double>();

            ReadFile(ref allPoints);
            MakeHulls(ref allPoints, ref convexHullPoints);
            GC.Collect();
            RemoveFirstZeroSquareHulls(ref convexHullPoints);
            CalculateDistancesBetweenHulls(ref convexHullPoints, ref distancesZ, ref numberOfLastZCoordinate);
            CalculateSquares(ref convexHullPoints, ref squares, ref numberOfLastZCoordinate);
            volume.SetVolume(distancesZ, squares, defformation);

            if (_informationModel.ToCorrectiveCoeff != 0)
            {
                SetVolumesByCoeff(ref volume, ref distancesZ, _informationModel.FromCorrectiveCoeff, _informationModel.ToCorrectiveCoeff, _informationModel.CorrectiveCoeff);
            }
            volume.TotalVolume = volume.VolumesBetweenHulls.Sum();

            string volumeStr = Math.Round(volume.TotalVolume, 4).ToString();
            CalculationResult = new CalculationResult(volumeStr);

            SortedDictionary<double, List<Point>> convexHullToHeightOfRez = CopySortDicByHeight(ref convexHullPoints, _informationModel.FillingHeight, _informationModel.ZeroPosition);

            if(_informationModel.TankType == TankType.Vertical)
            {

            }
            else
            {

            }
        }

        private SortedDictionary<double, List<Point>> CopySortDicByHeight(ref SortedDictionary<double, List<Point>> convexHull, double height, double zeroPosition)
        {
            SortedDictionary<double, List<Point>> newSortDic = new SortedDictionary<double, List<Point>>();
            double tempHeight = 0, beforeKey = 0;
            int iterator = 0;
            double heightRez = height + zeroPosition;

            foreach (var element in convexHull)
            {
                if (iterator == 0)
                {
                    beforeKey = element.Key;
                    iterator++;
                    continue;
                }

                if (tempHeight <= heightRez)
                {
                    tempHeight += Math.Abs(element.Key - beforeKey);
                    beforeKey = element.Key;
                    newSortDic.Add(element.Key, element.Value);
                    iterator++;
                }
                else
                {
                    break;
                }
            }

            return newSortDic;
        }

        private void SetVolumesByCoeff(ref Volume volume, ref List<double> distancesZ, double fromMeter, double toMeter, double coeff)
        {
            double from = 0;
            for (int i = 0; i < volume.VolumesBetweenHulls.Count; i++)
            {
                from += distancesZ[i];
                if (from >= fromMeter && from <= toMeter)
                {
                    volume.VolumesBetweenHulls[i] = volume.VolumesBetweenHulls[i] * coeff;
                }
            }
        }

        private void CalculateSquares(ref SortedDictionary<double, List<Point>> convexHullPoints, ref List<double> squares, ref int numberOfLastZCoordinate)
        {
            int numberOfCurrentZCoordinate = 0;
            foreach (KeyValuePair<double, List<Point>> listOfPoints in convexHullPoints)
            {
                squares.Add(GaussSquare.FindSquare(listOfPoints.Value));
                numberOfCurrentZCoordinate++;
                if (numberOfCurrentZCoordinate == numberOfLastZCoordinate + 1) break;
            }
        }

        private void CalculateDistancesBetweenHulls(ref SortedDictionary<double, List<Point>> convexHullPoints, ref List<double> distancesZ, ref int numberOfLastZCoordinate)
        {
            double previousZCoordinate = 0, distanceForVolume = 0;
            foreach (KeyValuePair<double, List<Point>> listOfPoints in convexHullPoints)
            {
                if (numberOfLastZCoordinate == 0)
                {
                    previousZCoordinate = listOfPoints.Key;
                    numberOfLastZCoordinate++;
                    continue;
                }
                if (distanceForVolume <= _informationModel.FillingHeight + _informationModel.ZeroPosition)
                {
                    distancesZ.Add(Math.Abs(listOfPoints.Key - previousZCoordinate));
                    distanceForVolume += Math.Abs(listOfPoints.Key - previousZCoordinate);
                    previousZCoordinate = listOfPoints.Key;
                    numberOfLastZCoordinate++;
                }
                else
                {
                    break;
                }
            }
        }

        private void RemoveFirstZeroSquareHulls(ref SortedDictionary<double, List<Point>> convexHullPoints)
        {
            List<double> removeFirstKeys = new List<double>();
            foreach (KeyValuePair<double, List<Point>> listOfPoints in convexHullPoints)
            {
                if (GaussSquare.FindSquare(listOfPoints.Value) == 0)
                {
                    removeFirstKeys.Add(listOfPoints.Key);
                }
                else
                {
                    break;
                }
            }
            if (removeFirstKeys.Count != 0)
            {
                foreach (double keyZ in removeFirstKeys)
                {
                    convexHullPoints.Remove(keyZ);
                }
            }
        }

        private void MakeHulls(ref SortedDictionary<double, List<Point>> allPoints, ref SortedDictionary<double, List<Point>> convexHullPoints)
        {
            List<Point> listOfPointsAfterHull;

            if(_informationModel.AlgorithmHullType == AlgorithmHullType.Andrew)
            {
                AndrewConvexHull andrewConvexHull = new AndrewConvexHull();
                foreach (KeyValuePair<double, List<Point>> point in allPoints)
                {
                    listOfPointsAfterHull = andrewConvexHull.CreateConvexHull(point.Value, _informationModel.MaxDistBetweenPoints, _informationModel.MaxDepth);
                    convexHullPoints.Add(listOfPointsAfterHull[0].Z, listOfPointsAfterHull);
                }
            }
            else if(_informationModel.AlgorithmHullType == AlgorithmHullType.Graham)
            {
                GrahamScanConvexHull grahamScanConvexHull = new GrahamScanConvexHull();
                foreach (KeyValuePair<double, List<Point>> point in allPoints)
                {
                    listOfPointsAfterHull = grahamScanConvexHull.GrahamScanCompute(point.Value, _informationModel.MaxDistBetweenPoints, _informationModel.MaxDepth);
                    convexHullPoints.Add(listOfPointsAfterHull[0].Z, listOfPointsAfterHull);
                }
            }
            
            allPoints.Clear();
        }

        private void ReadFile(ref SortedDictionary<double, List<Point>> allPoints)
        {
            string X, Y, Z;
            string fileLine;
            try
            {
                using (StreamReader sr = new StreamReader(_fileConnectionString, System.Text.Encoding.Default))
                {
                    while ((fileLine = sr.ReadLine()) != null)
                    {
                        string[] numbers = fileLine.Split(' ');
                        X = numbers[2].Replace(".", ",");
                        Y = numbers[3].Replace(".", ",");
                        Z = numbers[4].Replace(".", ",");

                        Point point = new Point(Convert.ToDouble(X), Convert.ToDouble(Y), Convert.ToDouble(Z));

                        List<Point> pointListWithSpecialZCoordinate = new List<Point>();

                        if (allPoints.ContainsKey(point.Z))
                        {
                            allPoints.TryGetValue(point.Z, out pointListWithSpecialZCoordinate);
                            pointListWithSpecialZCoordinate.Add(point);
                            allPoints[point.Z] = pointListWithSpecialZCoordinate;
                        }
                        else
                        {
                            allPoints.Add(point.Z, null);
                            pointListWithSpecialZCoordinate.Add(point);
                            allPoints[point.Z] = pointListWithSpecialZCoordinate;
                        }
                    }
                }
            }
            catch
            {
                throw new WrongFileStructureException();
            }
        }
    }
}
