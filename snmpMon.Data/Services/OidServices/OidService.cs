using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Engine.Services.OidServices
{
    public class OidService : IOidService
    {
        public string FreeMemoeryOid() => ".1.3.6.1.4.1.9.9.48.1.1.1.6.1";
        public string UsedMemoeryOid() => ".1.3.6.1.4.1.9.9.48.1.1.1.5.1";
        public string CpuUtilization() => ".1.3.6.1.4.1.9.9.109.1.1.1.1.8.1";
        public string SysUpTimeOid() => ".1.3.6.1.2.1.1.3.0";
        public string SysDetailsOid() => ".1.3.6.1.2.1.1";
        public string InterfacesDescOid() => ".1.3.6.1.2.1.2.2.1.2";
        public string InterfacesSpeed() => ".1.3.6.1.2.1.2.2.1.5";
        public string InteraceAdminStatusOid() => ".1.3.6.1.2.1.2.2.1.7";
        public string InterfaceOperStatusOid() => ".1.3.6.1.2.1.2.2.1.8";


    }
}
