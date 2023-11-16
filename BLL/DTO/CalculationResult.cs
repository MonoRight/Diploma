using System.Collections.Generic;

namespace BLL.DTO
{
    public class CalculationResult
    {
        public double Volume { get; set; }
        public List<double> VolumesBetweenHulls { get; set; }
        public double Fault { get; set; }

        //Table3.1
        public double CutNahily { get; set; }
        public double CutNahilyGraniciPohibki { get; set; }
        public double SeredniiRadius { get; set; }
        public double SeredniiRadiusGraniciPohibki { get; set; }
        public double ZagalnaDovjina { get; set; }
        public double ZagalnaDovjinaGraniciPohibki { get; set; }

        //Table3.3
        public double MistkistNekontrolovanoi { get; set; }
        public double MistkistNekontrolovanoiGraniciPohibki { get; set; }
        public double MistkistMertvoi { get; set; }
        public double MistkistMertvoiGraniciPohibki { get; set; }
        public double FullVolume { get; set; }
        public double FullVolumeGraniciPohibki { get; set; }

        //Table3.2
        public List<List<double>> VidhilenyaPoPererizam { get; set; }

        public List<Point> CentralPereriz { get; set; }
        public Point CentroidOfCentralPereriz { get; set; }

        public List<double> ListOfVolumesHorizontalCylindrPerSantimeter { get; set; }
    }
}
