using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Models
{
    public class DeviceAdd
    {
        public string DeviceName {  get; set; }
        public string IPAddress {  get; set; }
        public string CommunityString {  get; set; }
    }
}
