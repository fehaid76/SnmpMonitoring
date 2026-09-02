#pragma warning disable CA1416
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Extensions;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.Models;
using snmpMon.Resources.Constrols.Frames.FrameServices;
using snmpMon.Resources.Constrols.LabelControls;
using snmpMon.Services.AlertServices;
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
  
        private ObservableCollection<DeviceInterface> _interfaces = new();
        private SeriesCollection _seriesViews = new SeriesCollection();
        private ChartValues<double> _cpuUsageChartValues = new();
        private ChartValues<double> _memoryUsageChartValues = new();
        public SeriesCollection SeriesViews
        {
            get => _seriesViews;
            set
            {
                _seriesViews = value;
                OnPropertyChanged(nameof(SeriesViews));
            }
        }
        public ObservableCollection<DeviceInterface> Interfaces
        {
            get => _interfaces;
            set
            {
                _interfaces = value;
                OnPropertyChanged(nameof(Interfaces));
            }
        }

        private DeviceInfo _device = new();

        [ObservableProperty]
        private SystemInformation _systemInforamtion = new();

        private double _memoryUsage;

        private int _cpuUtilization;

        public TimeSpan UpTime
        {
            get => _systemInforamtion.UpTime;
            set
            {
                OnPropertyChanged(nameof(UpTime));
            }
        }

        public int CpuUtilization
        {
            get => _cpuUtilization;
            set
            {
                SetProperty(ref _cpuUtilization, value);
            }
        }

        public double MemoryUsage
        {
            get => _memoryUsage;
            set
            {
                _memoryUsage = value;
                OnPropertyChanged(nameof(MemoryUsage));
            }
        }



        public DeviceInfo Device
        {
            get => _device;
            set
            {

                if (SetProperty(ref _device, value))
                {
                    Task.Run(() =>
                    {

                        _startMonitoring();
                    });
                }
            }
        }


        private List<string> _labelsY = new();

        private List<string> _labelsX = new();

        public List<string> LabelsY
        {
            get => _labelsY;
            set
            {
                _labelsY = value;
                OnPropertyChanged(nameof(LabelsY));
            }
        }

        public List<string> LabelsX
        {
            get => _labelsX;
            set
            {
                _labelsY = value;
                OnPropertyChanged(nameof(LabelsX));
            }
        }

        public ICommand InterfaceCommand { get; set; }

        private readonly IDashboardSensors _dashboardSensors;
        private readonly IAlertNotify _alertNotify;
        private readonly IWindowService _windowService;
        public DeviceMonitoringVM(IDashboardSensors dashboardSensors, IAlertNotify alertNotify, IWindowService windowService)
        {

            _dashboardSensors = dashboardSensors;
            _windowService = windowService;
            _alertNotify = alertNotify;
            EventSubscribe();




            _dashboardSensors.OnDeviceStatusChanged += _dashboardSensors_OnDeviceStatusChanged;
            _dashboardSensors.OnResourceDeviceUsage += OnResourceDeviceUsage;

            GetSeriesCollection();


            InterfaceCommand = new RelayCommand<DeviceInterface>(d =>
            {
                if (d.AdminStatus == 2 || d.OperationStatus == 2)
                    return;
                _windowService.ShowInterfaceOverview(Device.IPAddress, Device.CommunityString, d.IfIndex, d.Description);

            });
        }

        private void _dashboardSensors_OnDeviceStatusChanged(object sender, Helper.EventHandle.DeviceStatusEventArgs e)
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
            _dashboardSensors.OnDeviceInterfaceReceived += OnDeviceInterfaceRecieved;
            _dashboardSensors.OnInterfaceStatusChanged += OnInterfaceStatusChanged;
            _dashboardSensors.OnUpTimeReceived += OnUpTimeReceived;
        }

        private void EventUnsubscribe()
        {
            _dashboardSensors.OnDeviceInterfaceReceived -= OnDeviceInterfaceRecieved;
            _dashboardSensors.OnInterfaceStatusChanged -= OnInterfaceStatusChanged;
            _dashboardSensors.OnUpTimeReceived -= OnUpTimeReceived;
        }

        private void OnDeviceStatusChangedToUp(string IPAddress, string CommunityString)
        {
            _alertNotify.ShowSuccess($"{Device.DeviceName} status changed to up");
            _startMonitoring();
            EventSubscribe();
            
        }





        private void OnDeviceStatusChangedToDown(string IPAddress)
        {
            EventUnsubscribe();

            _alertNotify.ShowErrors($"{Device.DeviceName} Status changed to down");
            SystemInforamtion.UpTime = new TimeSpan();
            CpuUtilization = 0;
            MemoryUsage = 0;
            _interfaces.Clear();

        }

        private void OnUpTimeReceived(TimeSpan obj)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SystemInforamtion.UpTime = obj;
            });

        }

        private void OnInterfaceStatusChanged(string arg1, int arg2)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Interfaces.Count > 0)
                {
                    for (int i = 0; i < Interfaces.Count; i++)
                    {
                        if (Interfaces[i].Description == arg1)
                        {

                            Interfaces[i].AdminStatus = arg2;
                            Interfaces[i].OperationStatus = arg2;
                        }
                    }
                }
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

        private void CpuUtilizationLogger(int value)
        {
            CpuUtilization = value;
            AddCpuUtilzationLineChartValue((double)value);
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
            _dashboardSensors.StartMonitoring(Device.IPAddress, Device.CommunityString);
        }


        public void Dispose()
        {
            //_dashboardSensors.Dispose();
        }


    }
}
