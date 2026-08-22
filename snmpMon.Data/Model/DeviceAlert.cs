using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Data.Model
{
    public class DeviceAlert
    {
        public string DeviceName {  get; set; }
        public string IPAddress {  get; set; }
        public string Message {  get; set; }
        public int Level {  get; set; }
        public string MessageTime {  get; set; }
    }
}
