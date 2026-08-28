using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Extensions
{
    public static class DoubleExtensions
    {
        public static bool IsBetween(this double value ,double startValue, double endValue)
        {
            if(value >= startValue && value <= endValue)
                return true;
            return false;
        }

        public static bool IsLargest(this double value, double comparingValue)
        {
            return value > comparingValue;
        }
    }
}
