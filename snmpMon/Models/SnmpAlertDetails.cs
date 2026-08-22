using CommunityToolkit.Mvvm.ComponentModel;
using snmpMon.Engine.Services.SnmpTraslatorServices.Enums;
using snmpMon.Engine.Services.SnmpTraslatorServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Models
{
    [INotifyPropertyChanged]
    public partial class SnmpAlertDetails : SnmpAlert
    {
        [ObservableProperty]

        private string _deviceName;





    }
}
