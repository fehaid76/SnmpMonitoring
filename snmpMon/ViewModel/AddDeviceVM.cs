using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using snmpMon.Models;
using snmpMon.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace snmpMon.ViewModel
{
    public class AddDeviceVM : ObservableObject
    {
        public event Action RequestCloseEvent; 
        public Device Device;
        

        private string _deviceName;
        private string _ipAddress;
        private string _comString;

        public string DeviceName
        {
            get => _deviceName;
            set
            {
                _deviceName = value;
                OnPropertyChanged(nameof(DeviceName));
            }
        }
        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                _ipAddress = value;
                OnPropertyChanged(nameof(IpAddress));
            }
        }
        public string ComString
        {
            get => _comString;
            set
            {
                _comString = value;
                OnPropertyChanged(nameof(ComString));
            }
        }

        public ICommand AddCMD {  get; set; }
        public ICommand CloseCMD {  get; set; }

        public AddDeviceVM()
        {
            CloseCMD = new RelayCommand(()=>
            {
                RequestClose();
            });

            AddCMD = new RelayCommand(() =>
            {
                Device = new Device()
                {
                    DeviceName = this.DeviceName,
                    IPAddress = this.IpAddress,
                    CommunityString = this.ComString

                };
            });

        }

        private void RequestClose() => RequestCloseEvent?.Invoke();
        
    }
}
