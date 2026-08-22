using CommunityToolkit.Mvvm.ComponentModel;
using snmpMon.Engine.Services.SnmpTraslatorServices.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace snmpMon.Engine.Services.SnmpTraslatorServices.Model
{

    public class SnmpAlert
    {

        public string IPAddress { get; set; }
        public string AlertMessage {  get; set; }
        public int AlertLevel {  get; set; }
        public string AlertTime {  get; set; }
        public string AlertSeverity {  get; set; }

    }
}
