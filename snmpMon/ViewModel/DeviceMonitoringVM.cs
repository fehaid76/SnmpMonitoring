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
using snmpMon.Services.AlertServices;
using snmpMon.Services.SensorServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace snmpMon.ViewModel
{
    public class DeviceMonitoringVM : ObservableObject
    {

        private ObservableCollection<DeviceInterface> _interfaces = new ObservableCollection<DeviceInterface>();
        private SeriesCollection _seriesViews = new SeriesCollection();
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
        private DeviceInfo _device = new DeviceInfo();
        private SystemInformation _sysInforamtion = new SystemInformation();
        private double _memoryUsage;
        private int _cpuUtilization; 

        public SystemInformation SysInformation
        {
            get => _sysInforamtion;
            set
            {
                
                SetProperty(ref _sysInforamtion, value);
            }
        }
        
        public TimeSpan UpTime
        {
            get => _sysInforamtion.UpTime;
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
                
                if(SetProperty(ref _device, value))
                {
                    Task.Run(() =>
                    {
                        
                        _startMonitoring();
                    });
                }
            }
        }

        private List<string> _labelsY = new List<string>();
        private List<string> _labelsX = new List<string>();

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

        public ICommand InterfaceCommand {  get; set; }

        private readonly IDashboardSensors _dashboardSensors;
        
        public DeviceMonitoringVM(IDashboardSensors dashboardSensors)
        {
            
            _dashboardSensors = dashboardSensors;
            _dashboardSensors.OnDeviceInterfaceReceived += _dashboardSensors_OnDeviceInterfaceRecieved;
            _dashboardSensors.OnMemoryUsageReceived += _dashboardSensors_OnMemoryUsageRecieved;
            _dashboardSensors.OnCPUutilizationReceived += _dashboardSensors_OnCPUutilizationRecieved;
            _dashboardSensors.OnInterfaceStatusChanged += _dashboardSensors_OnInterfaceStatusChanged;
            _dashboardSensors.OnUpTimeReceived += _dashboardSensors_OnUpTimeReceived;
            SysInformation.SystemName = Device.DeviceName;

            GetLabelsY();
            GetLabelsX();
            GetSeriesCollection();


            InterfaceCommand = new RelayCommand<DeviceInterface>(d =>
            {
                MessageBox.Show(d.IfIndex);
                
            });
        }



        private void _dashboardSensors_OnUpTimeReceived(TimeSpan obj)
        {
            SysInformation.UpTime = obj;

        }

        private void _dashboardSensors_OnInterfaceStatusChanged(string arg1, int arg2)
        {
            if(Interfaces.Count > 0)
            {
                for (int i = 0; i < Interfaces.Count; i++)
                {
                    if (Interfaces[i].Description == arg1)
                    {
                        
                        Interfaces[i].AdminStatus = arg2; 
                    }
                }
            }
        }

        private void _dashboardSensors_OnDeviceInterfaceRecieved(List<DeviceInterface> obj)
        {

            Application.Current.Dispatcher.Invoke(() =>
            {
                if(Interfaces.Count == 0)
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

            });

        }

        private void _dashboardSensors_OnCPUutilizationRecieved(int obj)
        {
             CpuUtilization = obj;

        }

        private void _dashboardSensors_OnMemoryUsageRecieved(double obj)
        {
                MemoryUsage = obj;

        }

        private void GetLabelsY()
        {
            LabelsY.Add("10");
            LabelsY.Add("20");

            LabelsY.Add("30");
            LabelsY.Add("40");
            LabelsY.Add("50");

        }

        private void GetLabelsX()
        {
            LabelsY.Add("1");
            LabelsY.Add("2");

            LabelsY.Add("3");
            LabelsY.Add("4");
            LabelsY.Add("5");

        }
        private void GetSeriesCollection()
        {
            SeriesViews.Add(new LineSeries
            {
                Title = "CPU Usage",
                Values = new ChartValues<double> { 5, 7, 9, 12, 15, 20},
                StrokeThickness = 2,
            });
            //SeriesViews.Add(new LineSeries
            //{
            //    Title = "CPU Usage",
            //    Values = new ChartValues<double> { 9, 17, 22, 112, 155, 210 },
            //    StrokeThickness = 1,
            //});

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
