using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Engine.Helper.EventHandle
{
    public class InterfaceStatusEventArgs : EventArgs
    {
        public string IPAddress { get; set; }
        public string IfDescr { get; set; }
        public bool IsUp { get; set; }

        public InterfaceStatusEventArgs(string iPAddress, string ifDescr, bool IsUp)
        {
            IPAddress = iPAddress;
            IfDescr = ifDescr;
            this.IsUp = IsUp;
        }
    }
}
