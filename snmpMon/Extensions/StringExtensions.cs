using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Extensions
{
    public static class StringExtensions
    {
        public static string? SafeIntent(this string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;


            return string.Intern(value);
        }

        public static string AddDigit(this string value, string digit)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return value + $".{digit}";
            return value;
        }
    }
}
