using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Data.Model
{
    public class DeviceEndpoint
    {
        public int ID {  get; set; }
        public string DeviceName {  get; set; }
        public string IPAddress {  get; set; }
    }
}
