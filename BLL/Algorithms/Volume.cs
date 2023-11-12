using System;
using System.Collections.Generic;

namespace BLL.Algorithms
{
    public class Volume
    {
        public List<double> VolumesBetweenHulls { get; set; }
        public double TotalVolume { get; set; }

        public void SetVolume(List<double> distances, List<double> squares, double linearTempCoeff)
        {
            VolumesBetweenHulls = new List<double>();
            for (int i = 0; i < distances.Count; i++)
            {
                VolumesBetweenHulls.Add(1.0 / 3.0 * distances[i] * (squares[i] + squares[i + 1] + Math.Sqrt(squares[i] * squares[i + 1])) * linearTempCoeff);
                TotalVolume += VolumesBetweenHulls[i];
            }
        }
    }
}
