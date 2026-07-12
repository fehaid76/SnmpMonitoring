using CommunityToolkit.Mvvm.ComponentModel;
using snmpMon.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Models
{

    public class DeviceVerification : ObservableObject
    {
        private VarificationStatus _pingVerification;
        private VarificationStatus _snmpProtocolVerification;
        private VarificationStatus _deviceVendorVerification;

        public VarificationStatus PingVerification
        {
            get => _pingVerification;
            set
            {
                _pingVerification = value;
                OnPropertyChanged(nameof(PingVerification));
            }
        }
        public VarificationStatus SNMPProtocolVerification
        {
            get => _snmpProtocolVerification;
            set
            {
                _snmpProtocolVerification = value;
                OnPropertyChanged(nameof(SNMPProtocolVerification));
            }
        }
        public VarificationStatus DeviceVendorVerification
        {
            get => _deviceVendorVerification;
            set
            {
                _deviceVendorVerification = value;
                OnPropertyChanged(nameof(DeviceVendorVerification));
            }
        }

    }
}
