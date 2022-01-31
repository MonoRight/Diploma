using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Exceptions
{
    public class GrahamScanConvexHullException : Exception
    {
        public GrahamScanConvexHullException()
        {
        }

        public GrahamScanConvexHullException(string message)
            : base(message)
        {
        }

        public GrahamScanConvexHullException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
