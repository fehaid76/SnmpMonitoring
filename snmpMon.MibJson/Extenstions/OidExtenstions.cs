using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.MibJson.Extenstions
{
    public static class OidExtenstions
    {
       
        public static string ToScalarOid(this string? baseOid)
        {

            return $"{baseOid}.0";
        }

        public static string ToFirstIntanceOid(this string baseOid)
        {
            return $"{baseOid}.1";
        }

        public static string GetOidIndex(this string oid)
        {
            return oid.Substring(oid.LastIndexOf('.') + 1);
        }

        public static string GetMemoryFree(this string value)
        {
            return "MemoryPoolFree";
        }

        public static string GetMemoryUsed(this string value)
        {
            return "MemoryPoolUsed";
        }


    }
}
