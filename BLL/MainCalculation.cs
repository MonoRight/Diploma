using BLL.Algorithms;
using BLL.DTO;
using BLL.Enums;
using BLL.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BLL
{
    public class MainCalculation
    {
        private readonly string _fileConnectionString;
        private readonly InformationModelDto _informationModel;
        private readonly AdditionalTablesModelDto _additionalTablesModelDto;
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
            CalculationResult = new CalculationResult();
            double midHeight;
            List<Point> centralPereriz;
            Point centroidOfCentralPereriz = null;
            List<double> listOfVolumesHorizontalCylindrPerSantimeter;
            double heightRez = _informationModel.FillingHeight + _informationModel.ZeroPosition;
            int kilkistShariv = Convert.ToInt32(_additionalTablesModelDto.T3_2KilkistShariv);
            int kilkistPereriziv = Convert.ToInt32(_additionalTablesModelDto.T3_2KilkistVerticalPeretiniv);

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

            CalculationResult.Volume = Math.Round(volume.TotalVolume, 8);
            CalculationResult.VolumesBetweenHulls = volume.VolumesBetweenHulls;
            CalculationResult.Fault = CalculateFault(_informationModel.NominalVolume, volume.TotalVolume);

            SortedDictionary<double, List<Point>> convexHullToHeightOfRez = CopySortDicByHeight(ref convexHullPoints, _informationModel.FillingHeight, _informationModel.ZeroPosition);
            keys = convexHullToHeightOfRez.Keys.ToList();
            midHeight = keys[keys.Count / 2]; //середня висота наповнення
            centralPereriz = convexHullToHeightOfRez[midHeight]; //центральний переріз

            if (_informationModel.TankType == TankType.Vertical)
            {
                centroidOfCentralPereriz = Centroid(centralPereriz); //середня точка централього перерізу
            }
            else
            {
                centroidOfCentralPereriz = Centroid(centralPereriz); //середня точка централього перерізу
                double radius = heightRez / 2; //радіус циліндра (м)
                Point centerOfCircleHorizontal = FindFarPointInPereriz(centroidOfCentralPereriz, centralPereriz); //центр першого кола (збоку найвіддаленіша точка в перерізі)
                double lengthOfRez = DistanceBetweenPoints(centerOfCircleHorizontal, centroidOfCentralPereriz) * 2; //довжина циліндра
                listOfVolumesHorizontalCylindrPerSantimeter = new List<double>();
                //double volume = Math.PI * radius * radius * lengthOfRez; //об'єм горизонтального циліндра
                double s = Math.PI * radius * radius;
                double stepRadianPerSantimetr = (360 * Math.PI / 180) / (heightRez * 100);

                for (int h = 0, i = 1; h < heightRez * 100; h++, i++)
                {
                    double value = lengthOfRez * radius * radius * (stepRadianPerSantimetr * i - Math.Sin(stepRadianPerSantimetr * i)) / 2.0;
                    listOfVolumesHorizontalCylindrPerSantimeter.Add(value);
                }
            }

            CalculationResult.CentralPereriz = centralPereriz;
            CalculationResult.CentroidOfCentralPereriz = centroidOfCentralPereriz;

            FillTable31(ref convexHullToHeightOfRez, centralPereriz, centroidOfCentralPereriz, defformation);
            FillTable33(volume, distancesZ);

            if (_informationModel.TankType == TankType.Vertical)
            {
                FillTable32(kilkistShariv, kilkistPereriziv, ref convexHullToHeightOfRez, centralPereriz, centroidOfCentralPereriz, heightRez);
            }
        }

        private List<Point> FindTwoPointsInTheListWithSpecifiedX(List<Point> points, double x)
        {
            List<Point> twoPoints = new List<Point>();

            foreach (var p in points)
            {
                if (p.X == x && twoPoints.Count != 2)
                {
                    twoPoints.Add(p);
                }
            }

            if (twoPoints.Count < 2)
            {
                twoPoints.Clear();
                Point maxY = FindMaxY(points);
                Point minY = FindMinY(points);
                maxY.X = x;
                minY.X = x;
                Point firstP = FindTheNearestPoint(points, maxY);
                Point secondP = FindTheNearestPoint(points, minY);
                twoPoints.Add(firstP);
                twoPoints.Add(secondP);

                return twoPoints;
            }
            else
            {
                return twoPoints;
            }
        }

        private Point FindTheNearestPoint(List<Point> points, Point point)
        {
            double dist = 999;
            Point nearestPoint = new Point();

            foreach (var p in points)
            {
                if (DistanceBetweenPoints(p, point) < dist)
                {
                    dist = DistanceBetweenPoints(p, point);
                    nearestPoint = p;
                }
            }

            return nearestPoint;
        }

        private Point FindMaxY(List<Point> points)
        {
            Point point = new Point();
            double y = -99;

            foreach (var p in points)
            {
                if (y < p.Y)
                {
                    y = p.Y;
                    point = p;
                }
            }

            return point;
        }

        private Point FindMinY(List<Point> points)
        {
            Point point = new Point();
            double y = 99;

            foreach (var p in points)
            {
                if (y > p.Y)
                {
                    y = p.Y;
                    point = p;
                }
            }

            return point;
        }
        private Point FindMinX(List<Point> points)
        {
            Point point = new Point();
            double x = 99;

            foreach (var p in points)
            {
                if (x > p.X)
                {
                    x = p.X;
                    point = p;
                }
            }

            return point;
        }

        private Point FindMaxX(List<Point> points)
        {
            Point point = new Point();
            double x = -99;

            foreach (var p in points)
            {
                if (x < p.X)
                {
                    x = p.X;
                    point = p;
                }
            }

            return point;
        }

        private void FillTable32(int kilkistShariv, int kilkistPereriziv, ref SortedDictionary<double, List<Point>> points, List<Point> centralPereriz, Point centroidOfCentralPereriz, double heightRez)
        {
            List<double> listHordsLengthsCircle = new List<double>();
            List<List<Point>> allPerepizi = new List<List<Point>>(); //список перерізів з надлишковими перерізами
            List<List<Point>> pererizi = new List<List<Point>>(); //список перерізів по шарах
            List<List<double>> vidhilenyaPoPererizam = new List<List<double>>(); //таблиця 3.2 для вертикального резервуару
            double r = GetRadiusCentralPereriz(centralPereriz, centroidOfCentralPereriz);
            double diametr = 2 * r;
            double step = diametr / (kilkistPereriziv + 1);
            double heightOfPereriz = heightRez / (2 * 3 * kilkistShariv); // відстань між центрами підшарів
            double tempHeight = 0, zCoordinateSaved = 0;
            double diameterLengthPereriz = 0;
            bool flag = true;
            int k = 0;

            for (int i = 0, m = 1; i < kilkistPereriziv; i++)
            {
                double value = 2 * Math.Sqrt(step * m * (diametr - step * m));
                listHordsLengthsCircle.Add(value); //довжина хорди круга
                m++;
            }

            foreach (var p in points)
            {
                if (k == 0)
                {
                    zCoordinateSaved = p.Key;
                    k++;
                    continue;
                }

                if (tempHeight >= heightOfPereriz)
                {
                    tempHeight = 0;
                    allPerepizi.Add(p.Value);
                }
                else
                {
                    tempHeight += Math.Abs(zCoordinateSaved - p.Key);
                }
                zCoordinateSaved = p.Key;
            }

            foreach (var p in allPerepizi)
            {
                if (flag)
                {
                    pererizi.Add(p);
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }

            foreach (var p in pererizi)
            {
                Point maxX = FindMaxX(p);
                Point minX = FindMinX(p);
                diameterLengthPereriz = Math.Abs(maxX.X - minX.X);
                step = diameterLengthPereriz / (kilkistPereriziv + 1);
                List<double> listHordsPereriz = new List<double>();
                List<double> riznici = new List<double>();

                for (int i = 0, c = 1; i < kilkistPereriziv; i++, c++)
                {
                    List<Point> twoPoints = FindTwoPointsInTheListWithSpecifiedX(p, minX.X + step * c);
                    double hordDist = DistanceBetweenPoints(twoPoints[0], twoPoints[1]);
                    listHordsPereriz.Add(hordDist);
                }

                int j = 0;
                foreach (var hord in listHordsPereriz)
                {
                    double vidhilenya = hord - listHordsLengthsCircle[j];
                    riznici.Add(Math.Round(vidhilenya, 3));
                    j++;
                }
                vidhilenyaPoPererizam.Add(riznici);
            }

            if (vidhilenyaPoPererizam.Count >= 2)
            {
                vidhilenyaPoPererizam.RemoveAt(0); //видалення нижнього перерізу
                vidhilenyaPoPererizam.RemoveAt(vidhilenyaPoPererizam.Count - 1); //видалення верхнього перерізу
            }

            CalculationResult.VidhilenyaPoPererizam = vidhilenyaPoPererizam;
        }

        private void ResultsMertvoi(Volume volume, List<double> distancesZ)
        {
            double heightCurrent = 0, volumeMertvoi = 0, summ = 0, standartNeviznachennist = 0;
            List<double> volumesMertvoi = new List<double>();
            int n = 0;

            foreach (double h in distancesZ)
            {
                if (heightCurrent >= _informationModel.ZeroPosition + _informationModel.DeathHeigth)
                {
                    break;
                }
                heightCurrent += h;
                volumeMertvoi += volume.VolumesBetweenHulls[n];
                volumesMertvoi.Add(volumeMertvoi);
                n++;
            }

            if (volumesMertvoi.Count > 0)
            {
                CalculationResult.MistkistMertvoi = volumesMertvoi.Last();
            }
            else
            {
                CalculationResult.MistkistMertvoi = 0;
            }

            n = volumesMertvoi.Count;
            if (n != 0)
            {
                volumeMertvoi = volumesMertvoi.Sum() / n;
            }
            else
            {
                volumeMertvoi = 0;
            }

            for (int i = 0; i < n; i++)
            {
                summ += Math.Pow(volumesMertvoi[i] - volumeMertvoi, 2);
            }

            if (n != 0)
            {
                standartNeviznachennist = Math.Sqrt(summ / (n * (n - 1)));
                CalculationResult.MistkistMertvoiGraniciPohibki = standartNeviznachennist;
            }
            else
            {
                CalculationResult.MistkistMertvoiGraniciPohibki = 0;
            }
        }

        private void ResultsMistkistNekontrolovanoi(Volume volume, List<double> distancesZ)
        {
            if (_informationModel.ZeroPosition > 0)
            {
                double heightCurrent = 0, volumeNekontrol = 0, summ = 0, standartNeviznachennist = 0;
                List<double> volumesNekontrol = new List<double>();
                int n = 0;

                foreach (double h in distancesZ)
                {
                    if (heightCurrent >= _informationModel.ZeroPosition)
                    {
                        break;
                    }
                    heightCurrent += h;
                    volumeNekontrol += volume.VolumesBetweenHulls[n];
                    volumesNekontrol.Add(volumeNekontrol);
                    n++;
                }

                if (volumesNekontrol.Count > 0)
                {
                    CalculationResult.MistkistNekontrolovanoi = volumesNekontrol.Last();
                }
                else
                {
                    CalculationResult.MistkistNekontrolovanoi = 0;
                }


                n = volumesNekontrol.Count;
                if (n != 0)
                {
                    volumeNekontrol = volumesNekontrol.Sum() / n;
                }
                else
                {
                    volumeNekontrol = 0;
                }

                for (int i = 0; i < n; i++)
                {
                    summ += Math.Pow(volumesNekontrol[i] - volumeNekontrol, 2);
                }

                if (n != 0)
                {
                    standartNeviznachennist = Math.Sqrt(summ / (n * (n - 1)));
                    CalculationResult.MistkistNekontrolovanoiGraniciPohibki = standartNeviznachennist;
                }
                else
                {
                    CalculationResult.MistkistNekontrolovanoiGraniciPohibki = 0;
                }
            }
            else
            {
                CalculationResult.MistkistNekontrolovanoi = 0;
                CalculationResult.MistkistNekontrolovanoiGraniciPohibki = 0;
            }
        }

        private void ResultsVolume(Volume volume)
        {
            CalculationResult.FullVolume = volume.TotalVolume;
            double sumVol = 0, summ = 0, standartNeviznachennist = 0;
            List<double> volumesPerHeight = new List<double>();
            int k = 0, n = 0;
            foreach (var v in volume.VolumesBetweenHulls)
            {
                if (k == 0)
                {
                    k++;
                    volumesPerHeight.Add(v);
                }
                else
                {
                    volumesPerHeight.Add(v + volumesPerHeight.Last());
                }
            }

            n = volumesPerHeight.Count;
            if (n != 0)
            {
                sumVol = volumesPerHeight.Sum() / n;
            }
            else
            {
                sumVol = 0;
            }

            for (int i = 0; i < n; i++)
            {
                summ += Math.Pow(volumesPerHeight[i] - sumVol, 2);
            }

            if (n != 0)
            {
                standartNeviznachennist = Math.Sqrt(summ / (n * (n - 1)));
                CalculationResult.FullVolumeGraniciPohibki = standartNeviznachennist;
            }
            else
            {
                CalculationResult.FullVolumeGraniciPohibki = 0;
            }
        }

        private void FillTable33(Volume volume, List<double> distancesZ)
        {
            ResultsVolume(volume);
            ResultsMistkistNekontrolovanoi(volume, distancesZ);
            ResultsMertvoi(volume, distancesZ);
        }

        private void ResultsCutNahiluVertical(ref SortedDictionary<double, List<Point>> points)
        {
            List<double> allCuts = new List<double>();
            double sumCut = 0, currentCut = 0, summ = 0, standartNeviznachennist = 0;
            int k = 0, n = 0;
            KeyValuePair<double, List<Point>> downPereriz = points.First();
            KeyValuePair<double, List<Point>> upperPereriz = points.Last();
            Point downCenter = Centroid(downPereriz.Value);
            Point upperCenter = Centroid(upperPereriz.Value);
            Point thirdVreshina = new Point() { X = upperCenter.X, Y = upperCenter.Y, Z = downCenter.Z };
            double catet1 = DistanceBetweenPoints(thirdVreshina, upperCenter);
            double catet2 = DistanceBetweenPoints(thirdVreshina, downCenter) * 1000;
            CalculationResult.CutNahily = catet2 / catet1;


            KeyValuePair<double, List<Point>> before = default;
            foreach (var p in points)
            {
                if (k == 0)
                {
                    before = p;
                    k++;
                    continue;
                }

                Point currentCentroid = Centroid(p.Value);
                Point beforeCentroid = Centroid(before.Value);
                Point pointSearch = new Point() { X = currentCentroid.X, Y = currentCentroid.Y, Z = beforeCentroid.Z };
                catet1 = DistanceBetweenPoints(currentCentroid, pointSearch);
                catet2 = DistanceBetweenPoints(beforeCentroid, pointSearch);
                currentCut = catet2 / catet1;
                allCuts.Add(currentCut);
                sumCut += currentCut;
                before = p;
            }
            n = allCuts.Count;
            if (n != 0)
            {
                sumCut = sumCut / n;
            }
            else
            {
                sumCut = 0;
            }

            for (int i = 0; i < n; i++)
            {
                summ += Math.Pow(allCuts[i] - sumCut, 2);
            }

            if (n != 0)
            {
                standartNeviznachennist = Math.Sqrt(summ / (n * (n - 1)));
                CalculationResult.CutNahilyGraniciPohibki = standartNeviznachennist;
            }
            else
            {
                CalculationResult.CutNahilyGraniciPohibki = 0;
            }
        }

        private void ResultsDovjinaVertical(double r, double defformation)
        {
            CalculationResult.ZagalnaDovjina = 2 * Math.PI * r * 1000;
            CalculationResult.ZagalnaDovjinaGraniciPohibki = 2 * Math.PI * CalculationResult.SeredniiRadiusGraniciPohibki / defformation;
        }

        private void ResultsRadiusVertical(ref SortedDictionary<double, List<Point>> points, double r, double defformation)
        {
            List<double> allRadiuses = new List<double>();
            double sumRadius = 0, currentRadius = 0, summ = 0, standartNeviznachennist = 0;
            CalculationResult.SeredniiRadius = r * defformation * 1000;

            foreach (var p in points)
            {
                currentRadius = GetEveryRadius(p.Value);
                allRadiuses.Add(currentRadius);
                sumRadius += currentRadius;
            }
            int n = allRadiuses.Count;
            if (n != 0)
            {
                sumRadius = sumRadius / n;
            }
            else
            {
                sumRadius = 0;
            }

            for (int i = 0; i < n; i++)
            {
                summ += Math.Pow(allRadiuses[i] - sumRadius, 2);
            }

            if (n != 0)
            {
                standartNeviznachennist = Math.Sqrt(summ / (n * (n - 1)));
                CalculationResult.SeredniiRadiusGraniciPohibki = standartNeviznachennist * 1000 * defformation;
            }
            else
            {
                CalculationResult.SeredniiRadiusGraniciPohibki = 0;
            }
        }

        private double GetEveryRadius(List<Point> list)
        {
            double maxDist = 0, minDist = 0, tempDist = 0;
            Point centr = Centroid(list);

            foreach (var p in list)
            {
                tempDist = DistanceBetweenPoints(centr, p);
                if (maxDist == 0 && minDist == 0)
                {
                    maxDist = minDist = tempDist;
                }
                else if (tempDist > maxDist)
                {
                    maxDist = tempDist;
                }
                else if (tempDist < minDist)
                {
                    minDist = tempDist;
                }
            }

            return (maxDist + minDist) / 2;
        }

        private double GetRadiusCentralPereriz(List<Point> centralPereriz, Point centroidOfCentralPereriz)
        {
            double maxDist = 0, minDist = 0, tempDist = 0;

            foreach (var p in centralPereriz)
            {
                tempDist = DistanceBetweenPoints(centroidOfCentralPereriz, p);
                if (maxDist == 0 && minDist == 0)
                {
                    maxDist = minDist = tempDist;
                }
                else if (tempDist > maxDist)
                {
                    maxDist = tempDist;
                }
                else if (tempDist < minDist)
                {
                    minDist = tempDist;
                }
            }

            return (maxDist + minDist) / 2;
        }

        private void FillTable31(ref SortedDictionary<double, List<Point>> points, List<Point> centralPereriz, Point centroidOfCentralPereriz, double defformation)
        {
            if (_informationModel.TankType == TankType.Vertical)
            {
                ResultsRadiusVertical(ref points, GetRadiusCentralPereriz(centralPereriz, centroidOfCentralPereriz), defformation);
                ResultsDovjinaVertical(GetRadiusCentralPereriz(centralPereriz, centroidOfCentralPereriz), defformation);
                ResultsCutNahiluVertical(ref points);
            }
            else
            {
                double r = (_informationModel.FillingHeight + _informationModel.ZeroPosition) / 2 + _informationModel.DeathHeigth;
                int n = 0;
                CalculationResult.SeredniiRadius = r * defformation * 1000;
                CalculationResult.ZagalnaDovjina = 2 * CalculationResult.SeredniiRadius * Math.PI;
                KeyValuePair<double, List<Point>> downPereriz = points.First();
                foreach (var p in points)
                {
                    if (n == 5000)
                    {
                        downPereriz = p;
                        break;
                    }
                    n++;
                }

                KeyValuePair<double, List<Point>> upperPereriz = points.Last();
                Point downCenter = Centroid(downPereriz.Value);
                Point upperCenter = Centroid(upperPereriz.Value);
                Point thirdVreshina = new Point() { X = upperCenter.X, Y = upperCenter.Y, Z = downCenter.Z };
                double catet1 = DistanceBetweenPoints(thirdVreshina, upperCenter);
                double catet2 = DistanceBetweenPoints(thirdVreshina, downCenter) * 1000;
                CalculationResult.CutNahily = catet2 / catet1;

                double fault = CalculateFault(2 * _informationModel.DeathHeigth + _informationModel.FillingHeight + _informationModel.ZeroPosition, _informationModel.FillingHeight + _informationModel.ZeroPosition) / 100;
                CalculationResult.SeredniiRadiusGraniciPohibki = CalculationResult.SeredniiRadius * fault;
                CalculationResult.ZagalnaDovjinaGraniciPohibki = CalculationResult.ZagalnaDovjina * fault;
                CalculationResult.CutNahilyGraniciPohibki = CalculationResult.CutNahily * fault;
            }

        }

        private double DistanceBetweenPoints(Point a, Point b)
        {
            return Math.Sqrt(((a.X - b.X) * (a.X - b.X)) + ((a.Y - b.Y) * (a.Y - b.Y)) + ((a.Z - b.Z) * (a.Z - b.Z)));
        }

        private Point FindFarPointInPereriz(Point centralPointOfCentralPereriz, List<Point> listPoint)
        {
            double maxDist = 0, tempDist = 0, x = 0, y = 0, z = 0;

            foreach (var p in listPoint)
            {
                tempDist = DistanceBetweenPoints(centralPointOfCentralPereriz, p);
                if (maxDist == 0)
                {
                    maxDist = tempDist;
                    x = p.X;
                    y = p.Y;
                    z = p.Z;
                }
                else if (tempDist > maxDist)
                {
                    maxDist = tempDist;
                    x = p.X;
                    y = p.Y;
                    z = p.Z;
                }
            }

            return new Point() { X = x, Y = y, Z = z };
        }

        private Point Centroid(List<Point> listPoint)
        {
            List<Point> points = RewriteList(listPoint);
            double xMid, yMid;
            try
            {
                for (int i = 0; i < listPoint.Count; i++)
                {
                    if (i != listPoint.Count - 1)
                    {
                        xMid = (listPoint[i].X + listPoint[i + 1].X) / 2;
                        yMid = (listPoint[i].Y + listPoint[i + 1].Y) / 2;
                        points.Add(new Point() { X = xMid, Y = yMid });
                    }
                    else
                    {
                        xMid = (listPoint[i].X + listPoint[0].X) / 2;
                        yMid = (listPoint[i].Y + listPoint[0].Y) / 2;
                        points.Add(new Point() { X = xMid, Y = yMid });
                        break;
                    }
                }
            }
            catch { }

            xMid = points.Sum(x => x.X) / points.Count;
            yMid = points.Sum(y => y.Y) / points.Count;

            return new Point() { X = xMid, Y = yMid, Z = points.First().Z };
        }

        private List<Point> RewriteList(List<Point> list)
        {
            List<Point> points = new List<Point>();

            foreach (Point point in list)
            {
                points.Add(point);
            }
            return points;
        }

        private double CalculateFault(double nominal, double fact)
        {
            return 100 - (100 * fact / nominal);
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

            if (_informationModel.AlgorithmHullType == AlgorithmHullType.Andrew)
            {
                AndrewConvexHull andrewConvexHull = new AndrewConvexHull();
                foreach (KeyValuePair<double, List<Point>> point in allPoints)
                {
                    listOfPointsAfterHull = andrewConvexHull.CreateConvexHull(point.Value, _informationModel.MaxDistBetweenPoints, _informationModel.MaxDepth);
                    convexHullPoints.Add(listOfPointsAfterHull[0].Z, listOfPointsAfterHull);
                }
            }
            else if (_informationModel.AlgorithmHullType == AlgorithmHullType.Graham)
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
