using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Services.AlertServices
{
    public class AlertMessage
    {
        public string DeviceName {  get; set; }
        public string Message {  get; set; }
        public bool isSent {  get; set; }
    }
}
