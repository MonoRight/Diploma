using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public InformationModel(string name, double nominalVolume, double fillingHeight, double deathHeight, TankType tankType)
        {
            Name = name;
            NominalVolume = nominalVolume;
            FillingHeight = fillingHeight;
            DeathHeigth = deathHeight;
            TankType = tankType;
        }
    }
}
