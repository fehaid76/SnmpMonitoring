using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Data.Extensions
{
    public static class StringExtensions
    {
        public static string? SafeIntern(this string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;
            return string.Intern(value);
        }
    }
}
