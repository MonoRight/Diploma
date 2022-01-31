using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Exceptions
{
    public class WrongFileStructureException : Exception
    {
        public WrongFileStructureException()
        {
        }

        public WrongFileStructureException(string message)
            : base(message)
        {
        }

        public WrongFileStructureException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
