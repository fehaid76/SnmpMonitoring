using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Helper.EventHandle
{
    public class TrafficEventArgs : EventArgs
    {
        public long TrafficIn { get; set; }
        public long TrafficOut {  get; set; }
    }
}
