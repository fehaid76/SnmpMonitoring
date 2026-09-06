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




namespace snmpMon.ViewModels.CardViewModels
{
    public partial class CardsViewModel :ObservableObject , IDisposable
    {

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

            AverageLatencyViews = new ObservableCollection<ISeries>
            {
                new LineSeries<ObservablePoint>
                {
                    Values = LatencyValues,
                    Fill = new SolidColorPaint(SKColors.DodgerBlue.WithAlpha(35)),
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
                    Fill = new SolidColorPaint(SKColors.DodgerBlue.WithAlpha(35)),
                    GeometrySize = 0,
                    LineSmoothness = 0.4,
                    Stroke = new SolidColorPaint(SKColors.DodgerBlue){StrokeThickness = 2},
                },

                new LineSeries<ObservablePoint>
                {
                    Name = "Outbound (kbps)",
                    Values = OutTrafficValues,
                    Fill = new SolidColorPaint(SKColors.GreenYellow.WithAlpha(35)),
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





        private string getDeviceName(string ipAddress)
        {
            var devicesDictionary = Devices.ToDictionary(i => i.IPAddress, n => n.Name);
            if(devicesDictionary != null && devicesDictionary.TryGetValue(ipAddress, out var deviceName))
            {
                return deviceName;
            }
            return string.Empty;
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
