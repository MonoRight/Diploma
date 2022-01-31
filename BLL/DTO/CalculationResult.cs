using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class CalculationResult
    {
        public string Volume { get; private set; }

        public CalculationResult(string volume)
        {
            Volume = volume;
        }
    }
}
