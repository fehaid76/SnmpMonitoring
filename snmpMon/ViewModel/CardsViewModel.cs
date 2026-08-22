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
    public class CardsViewModel :ObservableObject , IDisposable
    {
        private ObservableCollection<SnmpAlertDetails> _alertDetails = new ObservableCollection<SnmpAlertDetails>();
        private ObservableCollection<Axis> _xAxesList = new ObservableCollection<Axis>();
        public ObservableCollection<Axis> XAxesList
        {
            get => _xAxesList;
            set
            {
                _xAxesList = value;
                OnPropertyChanged(nameof(XAxesList));
            }
        }
        private int co = 0;

        private  ObservableCollection<ObservablePoint> _latencyValues = new ObservableCollection<ObservablePoint>();
        public ObservableCollection<ObservablePoint> LatencyValues
        {
            get => _latencyValues;
            set
            {
                _latencyValues = value;
                OnPropertyChanged(nameof(LatencyValues));
            }
        }


        private ObservableCollection<ObservablePoint> _inTrafficValues = new ObservableCollection<ObservablePoint>();
        public ObservableCollection<ObservablePoint> InTrafficValues
        {
            get => _inTrafficValues;
            set
            {
                _inTrafficValues = value;
                OnPropertyChanged(nameof(InTrafficValues));
            }
        }

        private ObservableCollection<ObservablePoint> _outTrafficValues = new ObservableCollection<ObservablePoint>();
        public ObservableCollection<ObservablePoint> OutTrafficValues
        {
            get => _outTrafficValues;
            set
            {
                _outTrafficValues = value;
                OnPropertyChanged(nameof(OutTrafficValues));
            }
        }

        public ObservableCollection<ISeries> _seriesViews2 = new ObservableCollection<ISeries>();

        public ObservableCollection<ISeries> SeriesViews2
        {
            get => _seriesViews2;
            set
            {
                _seriesViews2 = value;
                OnPropertyChanged(nameof(SeriesViews2));
            }
        }


        public ObservableCollection<ISeries> _inTrafficViews = new ObservableCollection<ISeries>();

        public ObservableCollection<ISeries> InTrafficViews
        {
            get => _inTrafficViews;
            set
            {
                _inTrafficViews = value;
                OnPropertyChanged(nameof(InTrafficViews));
            }
        }



        private List<Device> _devices = new List<Device>();

        public Paint LegendTextPaint { get; set; } = new SolidColorPaint
        {
            Color = SKColors.White,
            SKTypeface = SKTypeface.FromFamilyName("/Resources/Fonts/#Google Sans"),
        };


        public List<Device> Devices
        {
            get => _devices;
            set
            {
                _devices = value;
                OnPropertyChanged(nameof(Devices));
            }
        }

        private bool _isRefereshClicked = false;
        private string _topConsumerDevice;
        private double _averageLatency;
        private int _devicesCount;
        private int _alertsCount;

        public int AlertCount
        {
            get => _alertsCount;
            set
            {
                _alertsCount = value;
                OnPropertyChanged(nameof(AlertCount));
            }
        }
        public string TopConsumerDevice
        {
            get => _topConsumerDevice;
            set
            {
                _topConsumerDevice = value;
                OnPropertyChanged(nameof(TopConsumerDevice));
            }
        }
        public double AverageLatency
        {
            get => _averageLatency;
            set
            {
                _averageLatency = value;
                OnPropertyChanged(nameof(AverageLatency));
            }
        }
        public int DevicesCount
        {
            get => _devicesCount;
            set
            {
                _devicesCount = value;
                OnPropertyChanged(nameof(DevicesCount));
            }
        }


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
            
            SeriesViews2 = new ObservableCollection<ISeries>
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
            
            _snmpListener.OnSnmpTrapRecieved += _snmpListener_OnSnmpTrapRecieved;
            _overviewServices.OnAverageNetworkLatency += _overviewServices_OnAverageNetworkLatency;
            _overviewServices.OnUpDevices += _overviewServices_OnUpDevices;
            _devicesServices.OnDeviceDeleted += _devicesServices_OnDeviceDeleted;
            _devicesServices.OnDeviceAdded += _devicesServices_OnDeviceAdded;
            _overviewServices.OnInboundKbps += _overviewServices_OnInboundKbps;
            _overviewServices.OnTopInboundDevice += _overviewServices_OnTopInboundDevice;
            _overviewServices.OnOutboundKbps += _overviewServices_OnOutboundKbps;
        }

        private void _snmpListener_OnSnmpTrapRecieved(string arg1, Engine.Services.SnmpTraslatorServices.Model.SnmpAlert arg2)
        {
            if (!string.IsNullOrWhiteSpace(arg1) && arg2 != null)
            {
                AlertCount++;
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
            _snmpListener.OnSnmpTrapRecieved -= _snmpListener_OnSnmpTrapRecieved;
            _overviewServices.OnAverageNetworkLatency -= _overviewServices_OnAverageNetworkLatency;
            _overviewServices.OnUpDevices -= _overviewServices_OnUpDevices;
            _devicesServices.OnDeviceDeleted -= _devicesServices_OnDeviceDeleted;
            _devicesServices.OnDeviceAdded -= _devicesServices_OnDeviceAdded;
            _overviewServices.OnInboundKbps -= _overviewServices_OnInboundKbps;
            _overviewServices.OnTopInboundDevice -= _overviewServices_OnTopInboundDevice;
            _overviewServices.OnOutboundKbps -= _overviewServices_OnOutboundKbps;
        }


        private void _overviewServices_OnOutboundKbps(double obj)
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

        private void _overviewServices_OnTopInboundDevice(string obj)
        {
            TopConsumerDevice = obj;
        }

        private void _overviewServices_OnInboundKbps(double obj)
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

        private void _devicesServices_OnDeviceAdded(Device obj)
        {
            if(obj != null)
                Devices.Add(obj);
        }

        private void clearLinerChart()
        {
            LatencyValues.Clear();
        }


        
        private void _devicesServices_OnDeviceDeleted(string obj)
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

        private void _overviewServices_OnUpDevices(int obj)
        {
            DevicesCount = obj;
        }

        private void _overviewServices_OnAverageNetworkLatency(double obj)
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
