using snmpMon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Helper.EventHandle
{
    public class TrafficEventArgs : EventArgs
    {
        
        public double TrafficInSpeed { get; set; }
        public int InterfaceSpeed {  get; set; }
        public double TrafficOutSpeed {  get; set; }
        public double TrafficInVolume {  get; set; }
        public double TrafficOutVolume {  get; set; }
        public double TotalBytes {  get; set; }
        public string Timestamp {  get; set; }
    }
}
