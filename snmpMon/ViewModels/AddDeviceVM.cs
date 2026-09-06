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
    public class AddDeviceVM : ObservableObject
    {
        public event Action RequestCloseEvent; 
        public Device Device;
        private ObservableCollection<Category> _categories = new();
        private bool _canAddDevice;
        private string _deviceName;
        private string _ipAddress;
        private string _comString;
        private string _deviceVendor;
        private Category _selectedCategoty;
        private int _categoryID;
        public int CategoryID
        {
            get => _categoryID;
            set
            {
                _categoryID = value;
                OnPropertyChanged(nameof(CategoryID));
            }
        }
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }

        public Category SelectedCategory
        {
            get => _selectedCategoty;
            set
            {
                _selectedCategoty = value;
                OnPropertyChanged(nameof(SelectedCategory));
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
        public bool CanAddDevice
        {
            get => _canAddDevice;
            set
            {
                _canAddDevice = value;
                OnPropertyChanged(nameof(CanAddDevice));
                
            }
        }

        public string DeviceVendor
        {
            get => _deviceVendor;
            set
            {
                _deviceVendor = value;
                OnPropertyChanged(nameof(DeviceVendor));
            }
        }

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
