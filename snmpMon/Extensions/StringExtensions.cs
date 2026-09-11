using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public static int GetOidsCount(this string value)
        {
            return ConvertToOidList(value).Count;
        }

        public static List<string> ConvertToOidList(this string value)
        {
            string pattern = @"\d+(?:\.\d+){2,}";
            var oidGroup = Regex.Matches(value, pattern)
                .Cast<Match>()
                .Select(m => m.Value)
                .ToList();

            return oidGroup;
            
        }
        public static string AddDecimalToInt(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;
            string pattern = @"(?<![\d.])\b\d+\b(?![\d.])";
            return Regex.Replace(value, pattern, match => match.Value + ".0");

        }
        public static string ReplaceSpaces(this string value)
        {
            return value?.Replace(" ", string.Empty);
        }

        
    }
}
