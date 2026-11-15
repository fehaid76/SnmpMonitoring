using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using snmpMon.Core.Services;
using snmpMon.Data.Model;
using snmpMon.Models;
using snmpMon.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace snmpMon.ViewModels
{
    public partial class AddDeviceVM : ObservableObject
    {
        public event Action RequestCloseEvent; 
        public Device Device;
        [ObservableProperty]
        private ObservableCollection<Category> _categories = new();
        [ObservableProperty]
        private bool _canAddDevice;
        [ObservableProperty]
        private string _deviceName;
        [ObservableProperty]
        private string _ipAddress;
        [ObservableProperty]
        private string _comString;
        [ObservableProperty]
        private string _deviceVendor;
        [ObservableProperty]
        private Category _selectedCategoty;
        [ObservableProperty]
        private int _categoryID;



        public IRelayCommand AddCMD {  get; set; }
        public ICommand CloseCMD {  get; set; }

        private readonly IDeviceVerficationServices _verficationServices;
        private readonly IDevicesServices _devicesServices;
        public AddDeviceVM(IDeviceVerficationServices verficationServices, IDevicesServices devicesServices)
        {
            _verficationServices = verficationServices;
            _devicesServices = devicesServices;

            _ = GetCategories();

            _verficationServices.OnDeviceVendorRecieved += _verficationServices_OnDeviceVendorRecieved;
            _verficationServices.OnSnmpEnabled += OnSnmpEnabled;
            CanAddDevice = false;
            CloseCMD = new RelayCommand(()=>
            {
                RequestClose();
            });

            AddCMD = new RelayCommand(() =>
            {
                Device device = new Device()
                {
                    Name = this.DeviceName,
                    IPAddress = this.IpAddress,
                    CommunityString = this.ComString,
                    Vendor = this.DeviceVendor,
                    CategoryID = this.CategoryID,
                };
                Device = device;
                
            }, CanExecute);



        }

        private async Task GetCategories()
        {
            var categoriesList = await _devicesServices.GetAllCategoriesAsync();
            Application.Current.Dispatcher.Invoke(() =>
            {
                
                if (categoriesList.Count > 0)
                {
                    foreach (var category in categoriesList)
                    {
                        Categories.Add(category);
                    }
                }
            });
        }

        private void OnSnmpEnabled(bool obj)
        {
            CanAddDevice = obj;
            AddCMD.NotifyCanExecuteChanged();
        }

        private bool CanExecute()
        {
            
            return CanAddDevice;
        }
        private void _verficationServices_OnDeviceVendorRecieved(string obj)
        {
            if(!string.IsNullOrEmpty(obj))
                DeviceVendor = obj;
        }

        private void RequestClose() => RequestCloseEvent?.Invoke();
        
    }
}
