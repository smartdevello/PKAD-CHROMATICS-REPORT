using System;
using System.Collections.Generic;
using System.Text;

namespace PKAD_CHROMATICS_REPORT
{
    public class Helper
    {        public static double DegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180.0) * degrees;
            return radians;
        }
    }
}
