#pragma warning disable CA1416
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using snmpMon.Models;
using snmpMon.Services.SensorServices;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace snmpMon.ViewModel
{
    public class InterfaceOverviewViewModel : ObservableObject
    {
        private ObservableCollection<InterfaceDetails> _interfaceDetails = new ObservableCollection<InterfaceDetails>();
        private ObservableCollection<ObservablePoint> _trafficInSpeed = new ObservableCollection<ObservablePoint>();
        private ObservableCollection<ObservablePoint> _trafficOutSpeed = new ObservableCollection<ObservablePoint>();
        private ObservableCollection<ObservablePoint> _trafficTotalSpeeds = new ObservableCollection<ObservablePoint>();
        private ObservableCollection<ISeries> _trafficInVolumeViews = new ObservableCollection<ISeries>();
        private ObservableCollection<Axis> _xAxesList = new ObservableCollection<Axis>();
        public Paint LegendTextPaint { get; set; } = new SolidColorPaint
        {
            Color = SKColor.Parse("#E8E3E4"),
            SKTypeface = SKTypeface.FromFamilyName("/Resources/Fonts/#Google Sans"),
        };
        public ObservableCollection<Axis> XAxesList
        {
            get => _xAxesList;
            set
            {
                _xAxesList = value;
                OnPropertyChanged(nameof(XAxesList));
            }
        }
        public ObservableCollection<ISeries> TrafficSpeedsViews
        {
            get => _trafficInVolumeViews;
            set
            {
                _trafficInVolumeViews = value;
                OnPropertyChanged(nameof(TrafficSpeedsViews));
            }
        }
        public ObservableCollection<ObservablePoint> TrafficInSpeed
        {
            get => _trafficInSpeed;
            set
            {
                _trafficInSpeed = value;
                OnPropertyChanged(nameof(TrafficInSpeed));
            }
        }

        public ObservableCollection<ObservablePoint> TrafficOutSpeed
        {
            get => _trafficOutSpeed;
            set
            {
                _trafficInSpeed = value;
                OnPropertyChanged(nameof(TrafficOutSpeed));
            }
        }

        public ObservableCollection<ObservablePoint> TrafficTotalSpeeds
        {
            get => _trafficTotalSpeeds;
            set
            {
                _trafficInSpeed = value;
                OnPropertyChanged(nameof(TrafficTotalSpeeds));
            }
        }

        public ObservableCollection<InterfaceDetails> InterfaceDetails
        {
            get => _interfaceDetails;
            set
            {
                _interfaceDetails = value;
                OnPropertyChanged(nameof(InterfaceDetails));
            }
        }

        private string _ifDesc;
        public string IpAddress {  get; set; }
        public string CommunityString {  get; set; }
        public string IfIndex {  get; set; }
        public string IfDesc
        {
            get => _ifDesc;
            set
            {
                _ifDesc = value;
                OnPropertyChanged(nameof(IfDesc));
            }
        }
        private long _previousTrafficIn;
        private long _previousTrafficOut;
        public ICommand ClearCommand {  get; set; }
        private readonly IInterfacesSensors _interfacesSensors;

        public InterfaceOverviewViewModel(IInterfacesSensors interfacesSensors)
        {
            _interfacesSensors = interfacesSensors;

            _interfacesSensors.TrafficDataReady += HandleTrafficDataReady;

            prepareLineChart();

            ClearCommand = new RelayCommand(() =>
            {
                LienChartClear();
            });
        }

        private void LienChartClear()
        {
            if (TrafficSpeedsViews.Count > 0)
                TrafficSpeedsViews.Clear();
        }

        private void prepareLineChart()
        {
            TrafficSpeedsViews = new ObservableCollection<ISeries>
            {
                new LineSeries<ObservablePoint>
                {
                    Name = "Traffic In Speed",
                    Values = TrafficInSpeed,
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0.4,
                    Stroke = new SolidColorPaint(SKColor.Parse("#BD1E2B")){StrokeThickness = 2},
                },

                new LineSeries<ObservablePoint>
                {
                    Name = "Traffic Out Speed",
                    Values = TrafficOutSpeed,
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0.4,
                    Stroke = new SolidColorPaint(SKColor.Parse("#B9CB1C")){StrokeThickness = 2},
                },

                new LineSeries<ObservablePoint>
                {
                    Name = "Traffic Total Speeds",
                    Values = TrafficTotalSpeeds,
                    Fill = new SolidColorPaint(SKColor.Parse("#E9E9E9").WithAlpha(10)),
                    GeometrySize = 0,
                    LineSmoothness = 0.4,
                    Stroke = new SolidColorPaint(SKColor.Parse("#AAAAAE")){StrokeThickness = 3},
                },

            };

            XAxesList = new ObservableCollection<Axis>
            {
                new Axis
                {
                    Labeler = value => TimeSpan.FromSeconds(value).ToString(@"hh\:mm\:ss"),
                    MinStep = 5,
                    LabelsPaint = new SolidColorPaint(SKColors.White),
                    SeparatorsPaint = null,
                    LabelsRotation = 0,
                    TextSize = 10,
                }
            };

        }

        private void HandleTrafficDataReady(object sender, Helper.EventHandle.TrafficEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_previousTrafficIn == 0 && _previousTrafficOut == 0)
                {
                    _previousTrafficIn = e.TrafficIn;
                    _previousTrafficOut = e.TrafficOut;
                    return;
                }
                long diffIn = CalculateByteDifference(e.TrafficIn, _previousTrafficIn);
                long diffOut = CalculateByteDifference(e.TrafficOut, _previousTrafficOut);

                double trafficInVolume = ConvertBytesToKilobytes(diffIn);
                double trafficOutVolume = ConvertBytesToKilobytes(diffOut);

                double trafficInSpeed = CalculateSpeedInKilobitsPerSecond(diffIn);
                double trafficOutSpeed = CalculateSpeedInKilobitsPerSecond(diffOut);

                AddTrafficRecordToList(trafficInVolume, trafficOutVolume, trafficInSpeed, trafficOutSpeed);
                AddValuesToLineChart(trafficInSpeed, trafficOutSpeed, Math.Round(trafficInSpeed + trafficOutSpeed));
                CachePreviousTrafficValues(e.TrafficIn, e.TrafficOut);
            });
        }



        private void CachePreviousTrafficValues(long trafficIn, long trafficOut)
        {
            _previousTrafficIn = trafficIn;
            _previousTrafficOut = trafficOut;
        }
        private long CalculateByteDifference(long currentValue, long previousValue)
        {
            if (currentValue >= previousValue)
            {
                return (currentValue - previousValue);

            }

            return currentValue;

        }
        private double ConvertBytesToKilobytes(long bytes)
        {

            var val = (double)bytes / 1024;
            return Math.Round(val, 3);
        }

        public void Initialize(string ipAddress, string communityString, string ifIndex, string ifDesc)
        {

            IpAddress = ipAddress;
            CommunityString = communityString;
            IfIndex = ifIndex;
            IfDesc = ifDesc;
            _interfacesSensors.StartFetchInterfaceDetails(IpAddress, CommunityString, IfIndex);

        }

        private double CalculateSpeedInKilobitsPerSecond(long byteDifference)
        {
            if (byteDifference <= 0)
                return 0;

            double elapsedTime = 30.0;

            return Math.Round((byteDifference / elapsedTime * 8) / 1024, 3);
        }

        private void AddValuesToLineChart(double trafficInSpeed, double trafficOutSpeed, double trafficTotalSpeeds)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (trafficInSpeed > 0)
                {
                    double secondsToday = DateTime.Now.TimeOfDay.TotalSeconds;
                    var traficIn = new ObservablePoint(secondsToday, trafficInSpeed);
                    var trafficOut = new ObservablePoint(secondsToday, trafficOutSpeed);
                    var trafficTotal = new ObservablePoint(secondsToday, trafficTotalSpeeds);
                    TrafficInSpeed.Add(traficIn);
                    TrafficOutSpeed.Add(trafficOut);
                    TrafficTotalSpeeds.Add(trafficTotal);
                    if(TrafficInSpeed.Count > 60)
                    {
                        TrafficInSpeed.RemoveAt(0);
                        TrafficOutSpeed.RemoveAt(0);
                        TrafficTotalSpeeds.RemoveAt(0);
                    }
                }
            });
        }

        private void AddTrafficRecordToList(double trafficInVolume, double trafficOutVolume, double trafficInSpeed, double trafficOutSpeed)
        {
            InterfaceDetails intDeatils = new InterfaceDetails();
            intDeatils.InterfaceName = "Test";
            intDeatils.Ifindex = 1;
            intDeatils.DateTime = DateTime.Now.ToString("yyyy/MM/dd - HH:mm:ss");
            intDeatils.TrafficTotalVolume = Math.Round(trafficInVolume + trafficOutVolume, 3);
            intDeatils.TrafficInVolume = trafficInVolume;
            intDeatils.TrafficInSpeed = trafficInSpeed;
            intDeatils.TrafficOutSpeed = trafficOutSpeed;
            intDeatils.TrafficTotalSpeed = Math.Round(intDeatils.TrafficInSpeed + intDeatils.TrafficOutSpeed, 3);
            intDeatils.TrafficOutVolume = trafficOutVolume;

            InterfaceDetails.Insert(0, intDeatils);
            if(InterfaceDetails.Count > 60)
            {
                InterfaceDetails.RemoveAt(InterfaceDetails.Count - 1);
            }
        }
        
    }
}
