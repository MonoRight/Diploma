using BLL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class InformationModelDto
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
        public AlgorithmHullType AlgorithmHullType { get; set; }
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

        public InformationModelDto(string name, double nominalVolume, double fillingHeight, double deathHeight,
            TankType tankType, AlgorithmHullType algorithmHullType, double temperature, double linearTempCoeff, double maxDistBetweenPoints,
            double maxDepth, double zeroPosition, double correctiveCoeff, double fromCorrectiveCoeff, double toCorrectiveCoeff)
        {
            Name = name;
            NominalVolume = nominalVolume;
            FillingHeight = fillingHeight;
            DeathHeigth = deathHeight;
            TankType = tankType;
            AlgorithmHullType = algorithmHullType;
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
