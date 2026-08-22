using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Engine.Extensions
{
    public static class DictionaryExtensions
    {
        public static B GetmaxValue<T ,B>(this Dictionary<T, B> dictionary) where B : struct, System.IComparable<B>
        {
            if (dictionary == null || dictionary.Count == 0)
            {
                return default(B);
            }

            return dictionary.Values.Max();
        }
    }
}
