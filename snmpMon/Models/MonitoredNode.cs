using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Models
{
    public class MonitoredNode
    {
        public string IPAddress {  get; set; }
        public object MonitoringTask {  get; set; }
    }
}
