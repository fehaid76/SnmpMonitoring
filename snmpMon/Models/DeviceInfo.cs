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
        private int _deviceID;
        private string _deviceName;
        private string _vendor;
        private string _category;
        private int _portsCount;
        private bool _isConnected;
        private int _calculatinType;

        public int CalculatingType
        {
            get => _calculatinType;
            set
            {
                _calculatinType = value;
                OnPropertyChanged(nameof(CalculatingType));
            }
        }
        public string Category
        {
            get => _category;
            set
            {
                _category = value;
                OnPropertyChanged(nameof(Category));
            }
        }
        public int DeviceID
        {
            get => _deviceID;
            set
            {
                _deviceID = value;
                OnPropertyChanged(nameof(DeviceID));
            }
        }
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
