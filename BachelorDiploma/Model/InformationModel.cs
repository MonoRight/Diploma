using BLL.Enums;
using System.ComponentModel.DataAnnotations;

namespace BachelorDiploma.Model
{
    public class InformationModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public double NominalVolume { get; set; }
        [Required]
        public double FillingHeight { get; set; }
        [Required]
        public double DeathHeigth { get; set; }
        [Required]
        public TankType TankType { get; set; }
        [Required]
        public double Temperature { get; set; }
        [Required]
        public double LinearTempCoeff { get; set; }
        [Required]
        public double MaxDistBetweenPoints { get; set; }
        [Required]
        public double MaxDepth { get; set; }
        [Required]
        public double ZeroPosition { get; set; }
        [Required]
        public double CorrectiveCoeff { get; set; }
        [Required]
        public double FromCorrectiveCoeff { get; set; }
        [Required]
        public double ToCorrectiveCoeff { get; set; }

        public InformationModel(string name, double nominalVolume, double fillingHeight, double deathHeight,
            TankType tankType, double temperature, double linearTempCoeff, double maxDistBetweenPoints,
            double maxDepth, double zeroPosition, double correctiveCoeff, double fromCorrectiveCoeff, double toCorrectiveCoeff)
        {
            Name = name;
            NominalVolume = nominalVolume;
            FillingHeight = fillingHeight;
            DeathHeigth = deathHeight;
            TankType = tankType;
            Temperature = temperature;
            LinearTempCoeff = linearTempCoeff;
            MaxDistBetweenPoints = maxDistBetweenPoints;
            MaxDepth = maxDepth;
            ZeroPosition = zeroPosition;
            CorrectiveCoeff = correctiveCoeff;
            FromCorrectiveCoeff = fromCorrectiveCoeff;
            ToCorrectiveCoeff = toCorrectiveCoeff;
        }
    }
}
