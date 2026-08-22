using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Models
{
    public  class DeviceInterface : ObservableObject
    {
        public string Description {  get; set; }
        public string Type {  get; set; }
        public long Speed {  get; set; }
        private int _adminStatus;
        private int _operationStatus;
        public int AdminStatus
        {
            get => _adminStatus;
            set
            {
                if(SetProperty(ref _adminStatus, value))
                {
                    OnPropertyChanged(nameof(FormattedAdminStatus));
                }
            }
        }
        public int OperationStatus
        {
            get => _operationStatus;
            set
            {
                if(SetProperty(ref _operationStatus, value))
                {
                    OnPropertyChanged(nameof(FormattedOperationStatus));
                }
            }
        }
        public string IfIndex {  get; set; }

        public string FormattedSpeed
        {
            get
            {
                if(Speed >= 1000000000)
                {
                    double gbps = (double)Speed / 1000000000;
                    return $"{gbps} Gbps";
                }
                else if(Speed >= 1000000)
                {
                    double mbps = (double)Speed / 1000000;
                    return $"{mbps} Mbps";
                }
                else if (Speed >= 1000)
                {
                    double kbps = (double)Speed / 1000;
                    return $"{kbps} Kbps";

                }
                else
                {
                    return $"{Speed} bps";
                }
                    
            }

        }
        
        public string FormattedAdminStatus
        {
            get
            {
                if(AdminStatus == 1)
                {
                    return "Up";
                }
                else
                {
                    return "Down";
                }
            }
        }
        public string FormattedOperationStatus
        {
            get
            {
                if (OperationStatus == 1)
                {
                    return "Up";
                }
                else
                {
                    return "Down";
                }
            }
        }

    }
}
