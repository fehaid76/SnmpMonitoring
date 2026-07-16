using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using snmpMon.Services.SensorServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace snmpMon.Models
{
    public class DeviceInfo : ObservableObject
    {

        private string _deviceName;
        private string _vendor;
        private int _seccessCount;
        private int _errorsCount;
        private int _warnningCount;
        private int _portsCount;
        private bool _isConnected;

        public string DeviceName
        {
            get => _deviceName;
            set
            {
                _deviceName = value;
                OnPropertyChanged(nameof(DeviceName));
            }
        }
        public string Vendor
        {
            get => _vendor;
            set
            {
                _vendor = value;
                OnPropertyChanged(nameof(Vendor));
            }
        }
        public string IPAddress { get; set; }
        public int PortsCount
        {
            get => _portsCount;
            set
            {
                _portsCount = value;
                OnPropertyChanged(nameof(PortsCount));
            }
        }

        public int SuccessCount
        {
            get => _seccessCount;
            set
            {
                _seccessCount = value;
                OnPropertyChanged(nameof(SuccessCount));
            }
        }
        public int WarnningCount
        {
            get => _warnningCount;
            set
            {
                _warnningCount = value;
                OnPropertyChanged(nameof(WarnningCount));
            }
        }
        public int ErrorsCount
        {
            get => _errorsCount;
            set
            {
                _errorsCount = value;
                OnPropertyChanged(nameof(ErrorsCount));
            }
        }
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                OnPropertyChanged(nameof(IsConnected));
            }
        }
        public string CommunityString {  get; set; }
    }
}
