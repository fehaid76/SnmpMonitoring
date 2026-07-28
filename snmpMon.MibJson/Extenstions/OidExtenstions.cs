using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.MibJson.Extenstions
{
    public static class OidExtenstions
    {
        public static string ToScalarOid(this string baseOid)
        {
            return $"{baseOid}.0";
        }
    }
}
