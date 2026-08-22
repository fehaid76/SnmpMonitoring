using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Engine.Services.SnmpTraslatorServices.Enums
{
    public enum AlertLevels
    {
        Emergency = 0,
        Alert = 1,
        Critical,
        Error,
        Warning,
        Notice,
        Info,
        Debug
    }
}
