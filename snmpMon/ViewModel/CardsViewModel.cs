#pragma warning disable CA1416
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore.Defaults;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Newtonsoft.Json.Linq;
using ScottPlot.WPF;
using SkiaSharp;
using snmpMon.Data.Model;
using snmpMon.Services;
using snmpMon.Services.Agent;
using snmpMon.Services.WindowServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using LiveChartsCore.Painting;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.Models;
using System.Diagnostics;


namespace snmpMon.ViewModel
{
    public partial class CardsViewModel :ObservableObject , IDisposable
    {
        [ObservableProperty]
        private ObservableCollection<SnmpAlertDetails> _alertDetails = new ObservableCollection<SnmpAlertDetails>();
        [ObservableProperty]
        private ObservableCollection<Axis> _xAxesList = new ObservableCollection<Axis>();
        [ObservableProperty]
        private  ObservableCollection<ObservablePoint> _latencyValues = new ObservableCollection<ObservablePoint>();
        [ObservableProperty]
        private ObservableCollection<ObservablePoint> _inTrafficValues = new ObservableCollection<ObservablePoint>();
        [ObservableProperty]
        private ObservableCollection<ObservablePoint> _outTrafficValues = new ObservableCollection<ObservablePoint>();
        [ObservableProperty]
        public ObservableCollection<ISeries> _seriesViews = new ObservableCollection<ISeries>();
        [ObservableProperty]
        public ObservableCollection<ISeries> _inTrafficViews = new ObservableCollection<ISeries>();
        [ObservableProperty]
        private List<Device> _devices = new List<Device>();

        public Paint LegendTextPaint { get; set; } = new SolidColorPaint
        {
            Color = SKColors.White,
            SKTypeface = SKTypeface.FromFamilyName("/Resources/Fonts/#Google Sans"),
        };



        private bool _isRefereshClicked = false;
        [ObservableProperty]
        private string _topConsumerDevice;
        [ObservableProperty]
        private double _averageLatency;
        [ObservableProperty]
        private int _devicesCount;
        [ObservableProperty]
        private int _alertsCount;




        public ICommand ClearCommand { get; set; }


        private readonly IDevicesServices _devicesServices;
        private readonly IOverviewServices _overviewServices;
        private readonly IWindowService _windowService;
        private readonly ISnmpListener _snmpListener;
        public CardsViewModel(IDevicesServices devicesServices, IOverviewServices overviewServices, IWindowService windowService, ISnmpListener snmpListener)
        {

            
            _devicesServices = devicesServices;
            _overviewServices = overviewServices;
            _windowService = windowService;
            _snmpListener = snmpListener;

            eventSubsecrib();
            _startInterfacesTrafficProcess();
            
            SeriesViews = new ObservableCollection<ISeries>
            {
                new LineSeries<ObservablePoint>
                {
                    Values = LatencyValues,
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0.4,
                    Stroke = new SolidColorPaint(SKColors.DodgerBlue){StrokeThickness = 2},
                }
            };

            InTrafficViews = new ObservableCollection<ISeries>
            {
                new LineSeries<ObservablePoint>
                {
                    Name = "Inbound (kbps)",
                    Values = InTrafficValues,
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0.4,
                    Stroke = new SolidColorPaint(SKColors.DodgerBlue){StrokeThickness = 2},
                },

                new LineSeries<ObservablePoint>
                {
                    Name = "Outbound (kbps)",
                    Values = OutTrafficValues,
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0.4,
                    Stroke = new SolidColorPaint(SKColors.GreenYellow){StrokeThickness = 2},
                }
            };

            XAxesList = new ObservableCollection<Axis>
            {
                new Axis
                {
                    Labeler = value =>TimeSpan.FromSeconds(value).ToString(@"hh\:mm\:ss"),

                    MinStep = 5,
                    LabelsPaint = new SolidColorPaint(SKColors.White),
                    SeparatorsPaint = null,

                    LabelsRotation = 0,
                    TextSize = 10,
                    
                    
                }
            };

            _ = loadDevices();
            memoryCleaner();

            ClearCommand = new RelayCommand(() =>
            {
                if (_windowService.ShowConfirmDialog("Are you sure you want to clear data?","Confirm Dialog", MessageBoxIcon.Question))
                {
                    clearLinerChart();
                }
            });



        }

        
        private void _startInterfacesTrafficProcess()
        {
            _overviewServices.StartTrafficMonitoring();

        }



        private void eventSubsecrib()
        {
            
            _snmpListener.OnSnmpTrapRecieved += HandleSnmpTraps;
            _overviewServices.OnAverageNetworkLatency += HandleAverageNetworkLatency;
            _overviewServices.OnUpDevices += HandleUpDevices;
            _devicesServices.OnDeviceDeleted += OnDeviceDeleted;
            _devicesServices.OnDeviceAdded += OnDeviceAdded;
            _overviewServices.OnInboundKbps += HandleInboundKbps;
            _overviewServices.OnTopInboundDevice += HandleTopInboundDevice;
            _overviewServices.OnOutboundKbps += HandleOutboundKbps;
        }

        private void HandleSnmpTraps(string arg1, Engine.Services.SnmpTraslatorServices.Model.SnmpAlert arg2)
        {
            if (!string.IsNullOrWhiteSpace(arg1) && arg2 != null)
            {
                AlertsCount++;
            }
        }

        private string getDeviceName(string ipAddress)
        {
            var devicesDictionary = Devices.ToDictionary(i => i.IPAddress, n => n.Name);
            if(devicesDictionary != null && devicesDictionary.TryGetValue(ipAddress, out var deviceName))
            {
                return deviceName;
            }
            return string.Empty;
        }

        private void eventUnsubsecrib()
        {
            _snmpListener.OnSnmpTrapRecieved -= HandleSnmpTraps;
            _overviewServices.OnAverageNetworkLatency -= HandleAverageNetworkLatency;
            _overviewServices.OnUpDevices -= HandleUpDevices;
            _devicesServices.OnDeviceDeleted -= OnDeviceDeleted;
            _devicesServices.OnDeviceAdded -= OnDeviceAdded;
            _overviewServices.OnInboundKbps -= HandleInboundKbps;
            _overviewServices.OnTopInboundDevice -= HandleTopInboundDevice;
            _overviewServices.OnOutboundKbps -= HandleOutboundKbps;
        }


        private void HandleOutboundKbps(double obj)
        {
            if (DevicesCount == 0)
                return;

            Application.Current.Dispatcher?.Invoke(() =>
            {


                double secondsToday = DateTime.Now.TimeOfDay.TotalSeconds;
                if(OutTrafficValues.Count >= 30)
                {
                    var existingPoint = OutTrafficValues[0];
                    existingPoint.X = secondsToday;
                    existingPoint.Y = obj;

                    OutTrafficValues.RemoveAt(0);
                    OutTrafficValues.Add(existingPoint);

                }
                else
                {
                    OutTrafficValues.Add(new ObservablePoint(secondsToday, obj));
                }

            });
        }

        private void HandleTopInboundDevice(string obj)
        {
            TopConsumerDevice = obj;
        }

        private void HandleInboundKbps(double obj)
        {


            Application.Current.Dispatcher?.Invoke(() =>
            {
                if (DevicesCount == 0)
                    return;

                    double secondsToday = DateTime.Now.TimeOfDay.TotalSeconds;
                if(InTrafficValues.Count >= 30)
                {
                    var existingPoint = InTrafficValues[0];
                    existingPoint.X = secondsToday;
                    existingPoint.Y = obj;

                    InTrafficValues.RemoveAt(0);
                    InTrafficValues.Add(existingPoint);
                    
                }
                else
                {
                    InTrafficValues.Add(new ObservablePoint(secondsToday, obj));
                }

            });

        }

        private void OnDeviceAdded(Device obj)
        {
            if(obj != null)
                Devices.Add(obj);
        }

        private void clearLinerChart()
        {
            LatencyValues.Clear();
        }


        
        private void OnDeviceDeleted(string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                var tragetDevice = Devices.FirstOrDefault(d => d.IPAddress == obj);
                if (tragetDevice != null)
                {
                    Devices.Remove(tragetDevice);
                }
            }
        }

        private void HandleUpDevices(int obj)
        {
            DevicesCount = obj;
        }

        private void HandleAverageNetworkLatency(double obj)
        {
            if (DevicesCount == 0)
                return;
            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                AverageLatency = obj;
                updateSeries(obj);


            });

        }

        private void updateSeries(double value)
        {
            double secondsToday = DateTime.Now.TimeOfDay.TotalSeconds;
 
            if(LatencyValues.Count >= 30)
            {
                var existingPoint = LatencyValues[0];
                existingPoint.X = secondsToday;
                existingPoint.Y = value;

                LatencyValues.RemoveAt(0);
                LatencyValues.Add(existingPoint);
            }
            else
            {
                LatencyValues.Add(new ObservablePoint(secondsToday, value));
            }

            
        }



        private async Task loadDevices()
        {
            var devices = await _devicesServices.GetAllDevicesAsync();
            if(devices.Count > 0)
            {
                DevicesCount = devices.Count;

                _devices = devices;

                await startAveCalculating();

            }
        }
        private async Task startAveCalculating()
        {
            if(_devices.Count > 0)
            {
                await _overviewServices.StartNetworkAverageLatencyCalculateAsync(_devices);
            }
        }

        private void memoryCleaner()
        {
            _overviewServices.MemoryCleaner();
        }

        public void Dispose()
        {
            eventUnsubsecrib();
        }
    }
}
