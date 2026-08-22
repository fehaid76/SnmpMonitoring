using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Engine.Services.SnmpTraslatorServices.Model
{
    public class SnmpTrapPacket
    {
        public string TrapOid {  get; set; }

        public Dictionary<string, string> VarBinds { get; set; }
    }
}
