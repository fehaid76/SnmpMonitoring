using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Helper.Model
{
    public class ActionDisposableAggregation
    {
        public string IPAddress { get; set; }
        public ActionDisposable actionDisposable { get; set; }
    }
}
