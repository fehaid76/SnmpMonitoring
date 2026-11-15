#pragma warning disable CA1416
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Extensions;
using snmpMon.Engine.Helper.EventHandle;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.Helper.EventHandle;
using snmpMon.Models;
using snmpMon.Resources.Constrols.Frames.FrameServices;
using snmpMon.Resources.Constrols.LabelControls;
using snmpMon.Services.AlertServices;
using snmpMon.Services.DeviceServices;
using snmpMon.Services.SensorServices;
using snmpMon.Services.WindowServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using SystemInformation = snmpMon.Models.SystemInformation;



namespace snmpMon.ViewModels
{
    public partial class DeviceMonitoringVM : ObservableObject
    {

        [ObservableProperty]
        private ObservableCollection<DeviceInterface> _interfaces = new();
        [ObservableProperty]
        private SeriesCollection _seriesViews = new();
        private ChartValues<double> _cpuUsageChartValues = new();
        private ChartValues<double> _memoryUsageChartValues = new();

        private DeviceInfo _device = new();
        public DeviceInfo Device
        {
            get => _device;
            set
            {
                if(SetProperty(ref _device, value))
                {
                    _startMonitoring();
                    GetSystemDetails();


                }
            }
        }
        [ObservableProperty]
        private SystemInformation _systemInforamtion = new();
        [ObservableProperty]
        private double _memoryUsage;
        [ObservableProperty]
        private double _cpuUtilization;
        [ObservableProperty]
        private List<string> _labelsY = new();
        [ObservableProperty]
        private List<string> _labelsX = new();


        public ICommand InterfaceCommand { get; set; }

        private readonly IDashboardSensors _dashboardSensors;
        private readonly IAlertNotify _alertNotify;
        private readonly IWindowService _windowService;
        private readonly IDeviceInformationServices _deviceInformationServices;
        public DeviceMonitoringVM(IDashboardSensors dashboardSensors, IAlertNotify alertNotify, IWindowService windowService, IDeviceInformationServices deviceInformationServices)
        {

            _dashboardSensors = dashboardSensors;
            _windowService = windowService;
            _alertNotify = alertNotify;
            _deviceInformationServices = deviceInformationServices;
            EventSubscribe();


            GetSeriesCollection();


            InterfaceCommand = new RelayCommand<DeviceInterface>(d =>
            {
                if (d.AdminStatus == 2 || d.OperationStatus == 2)
                    return;
                _windowService.ShowInterfaceOverview(Device.IPAddress, Device.CommunityString, d.IfIndex, d.Description);

            });
            
        }

        private void GetSystemDetails()
        {
            Task.Run(async () =>
            {
                //var processDuration = TimeSpan.FromSeconds(5);
                var sysDetails = await _deviceInformationServices.GetSysDetails(Device.IPAddress, Device.CommunityString);
                AssignSystemDetails(sysDetails.sysName, sysDetails.sysLocation, sysDetails.sysStatus);
            });

        }

        private void AssignSystemDetails(string sysName, string sysLocation, bool sysStatus)
        {
            SystemInforamtion.SystemName = sysName;
            SystemInforamtion.Location = sysLocation;
            SystemInforamtion.IsConnected = sysStatus;

        }


        private void OnDeviceStatusChanged(object sender, Helper.EventHandle.DeviceStatusEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (e.DeviceStatus == Enums.DeviceStatus.Up)
                {
                    OnDeviceStatusChangedToUp(e.IPAddress, e.CommunityString);
                }
                else if (e.DeviceStatus == Enums.DeviceStatus.Down)
                {
                    OnDeviceStatusChangedToDown(e.IPAddress);
                }
            });
        }

        private void OnResourceDeviceUsage(object sender, Helper.EventHandle.ResourceUsageEventArgs e)
        {
            if(e != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MemoryUsageLogger(e.MemoryUsage);
                    CpuUtilizationLogger(e.CPUutilization);
                });
            }
        }

        private void EventSubscribe()
        {
            _dashboardSensors.OnDeviceStatusChanged += OnDeviceStatusChanged;
            _dashboardSensors.OnResourceDeviceUsage += OnResourceDeviceUsage;
            _dashboardSensors.OnDeviceInterfaceReceived += OnDeviceInterfaceRecieved;
            _dashboardSensors.OnInterfaceStatusChanged += OnInterfaceStatusChanged;
            _dashboardSensors.OnUpTimeReceived += OnUpTimeReceived;
        }

        private void OnInterfaceStatusChanged(object sender, InterfaceStatusEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Interfaces.Count > 0)
                {
                    if (Device.IPAddress == e.IPAddress)
                    {
                        for (int i = 0; i < Interfaces.Count; i++)
                        {
                            if (Interfaces[i].Description == e.IfDescr)
                            {

                                Interfaces[i].AdminStatus = e.IsUp? 1 : 2;
                                Interfaces[i].OperationStatus = e.IsUp ? 1 : 2;
                            }
                        }
                    }


                }
            });
        }

        private void EventUnsubscribe()
        {
            _dashboardSensors.OnDeviceInterfaceReceived -= OnDeviceInterfaceRecieved;
            _dashboardSensors.OnInterfaceStatusChanged -= OnInterfaceStatusChanged;
            _dashboardSensors.OnUpTimeReceived -= OnUpTimeReceived;
        }

        private void OnDeviceStatusChangedToUp(string IPAddress, string CommunityString)
        {
            EventSubscribe();
            _alertNotify.ShowSuccess($"{Device.DeviceName} status changed to up");
            SystemInforamtion.IsConnected = true;
            _startMonitoring();
            
            
        }





        private void OnDeviceStatusChangedToDown(string IPAddress)
        {
            EventUnsubscribe();

            _alertNotify.ShowErrors($"{Device.DeviceName} Status changed to down");
            SystemInforamtion.UpTime = new TimeSpan();
            CpuUtilization = 0;
            MemoryUsage = 0;
            SystemInforamtion.IsConnected = false;
            Interfaces.Clear();

        }

        private void OnUpTimeReceived(TimeSpan obj)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SystemInforamtion.UpTime = obj;
            });

        }

        private void OnDeviceInterfaceRecieved(List<DeviceInterface> obj)
        {

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Interfaces.Count == 0)
                {
                    foreach (var i in obj)
                    {
                        Interfaces.Add(i);
                    }
                    return;
                }

                var updatedDic = obj.ToDictionary(x => x.IfIndex);
                foreach (var existingInterface in Interfaces)
                {
                    ApplyInterfaceStatusUpdates(obj);
                }

            });

        }

        private void ApplyInterfaceStatusUpdates(List<DeviceInterface> groupOfIntrefaces)
        {
            var updatedDic = ConvertListToDictionaryBasedOnIfIndex(groupOfIntrefaces);
            foreach (var existingInterface in Interfaces)
            {
                if (updatedDic.TryGetValue(existingInterface.IfIndex, out var updatedItem))
                {
                    if (existingInterface.AdminStatus != updatedItem.AdminStatus)
                    {
                        existingInterface.AdminStatus = updatedItem.AdminStatus;
                    }

                    if (existingInterface.OperationStatus != updatedItem.OperationStatus)
                    {
                        existingInterface.OperationStatus = updatedItem.OperationStatus;
                    }
                }
            }
        }

        private Dictionary<string, DeviceInterface> ConvertListToDictionaryBasedOnIfIndex(List<DeviceInterface> groupOfIntrefaces)
        {
            return groupOfIntrefaces.ToDictionary(x => x.IfIndex);
        }



        private void MemoryUsageLogger(double value)
        {
            MemoryUsage = value;
            AddMemoryLineChartValue(value);
        }

        private void CpuUtilizationLogger(double value)
        {
            CpuUtilization = value;
            AddCpuUtilzationLineChartValue(value);
        }

        private void ChecKForChartValuesCount()
        {
            if (_memoryUsageChartValues.Count >= 50)
                _memoryUsageChartValues.RemoveAt(0);
            else if(_cpuUsageChartValues.Count >= 50)
                _cpuUsageChartValues.RemoveAt(0);
        }

        private void AddMemoryLineChartValue(double value)
        {
            _memoryUsageChartValues.Add(value);
            ChecKForChartValuesCount();
        }

        private void AddCpuUtilzationLineChartValue(double cpuUsage)
        {
            _cpuUsageChartValues.Add(cpuUsage);
            ChecKForChartValuesCount();

        }


        private void GetSeriesCollection()
        {
            SeriesViews.Add(new LineSeries
            {
                Title = "CPU Utilization",
                Values = _cpuUsageChartValues,
                StrokeThickness = 2,
            });
            SeriesViews.Add(new LineSeries
            {
                Title = "Memeory Usage",
                Values = _memoryUsageChartValues,
                StrokeThickness = 1,
            });

        }


        private void _startMonitoring()
        {

            _dashboardSensors.StartMonitoring(Device.IPAddress, Device.CommunityString, Device.Category);

            
        }


        public void Dispose()
        {
            //_dashboardSensors.Dispose();
        }


    }
}
