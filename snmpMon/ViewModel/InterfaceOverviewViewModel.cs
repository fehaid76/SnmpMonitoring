#pragma warning disable CA1416
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using snmpMon.Extensions;
using snmpMon.Models;
using snmpMon.Services.SensorServices;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace snmpMon.ViewModel
{
    public partial class InterfaceOverviewViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<InterfaceDetails> _interfaceDetails = new();
        [ObservableProperty]
        private ObservableCollection<ObservablePoint> _trafficInSpeed = new();
        [ObservableProperty]
        private ObservableCollection<ObservablePoint> _trafficOutSpeed = new();
        [ObservableProperty]
        private ObservableCollection<ObservablePoint> _trafficTotalSpeeds = new();
        [ObservableProperty]
        private ObservableCollection<ISeries> _trafficSpeedsViews = new();
        [ObservableProperty]
        private ObservableCollection<Axis> _xAxesList = new();

     

        public Paint LegendTextPaint { get; set; } = new SolidColorPaint
        {
            Color = SKColor.Parse("#E8E3E4"),
            SKTypeface = SKTypeface.FromFamilyName("/Resources/Fonts/#Google Sans"),
        };

        [ObservableProperty]
        private string _ifDesc;

        public string IpAddress {  get; set; }
        public string CommunityString {  get; set; }
        public string IfIndex {  get; set; }


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
                    Fill = new LinearGradientPaint(
                         new SKColor[] { SKColor.Parse("#E9E9E9").WithAlpha(90), SKColor.Parse("#E9E9E9").WithAlpha(0) },
                         new SKPoint(0.5f, 0f),
                         new SKPoint(0.5f, 5f)
    ),
                    GeometrySize = 0,
                    LineSmoothness = 5.4,
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
                AddTrafficRecordToList(e.TrafficInVolume, e.TrafficOutVolume, e.TrafficInSpeed, e.TrafficOutSpeed, e.InterfaceSpeed);
                AddValuesToLineChart(e.TrafficInSpeed, e.TrafficOutSpeed, Math.Round((double)e.TrafficInSpeed + (double)e.TrafficOutSpeed));
            });
        }

        public void Initialize(string ipAddress, string communityString, string ifIndex, string ifDesc)
        {
            
            IpAddress = ipAddress;
            CommunityString = communityString;
            IfIndex = ifIndex;
            IfDesc = ifDesc;
            _interfacesSensors.StartFetchInterfaceDetails(IpAddress, CommunityString, IfIndex);

        }


        private (ObservablePoint TrafficIn, ObservablePoint TrafficOut, ObservablePoint TrafficTotal) CreateTrafficPointsTuple(double trafficInSpeed, double trafficOutSpeed, double trafficTotalSpeeds)
        {
            double secondsToday = DateTime.Now.TimeOfDay.TotalSeconds;

            var trafficIn = new ObservablePoint(secondsToday, trafficInSpeed);
            var trafficOut = new ObservablePoint(secondsToday, trafficOutSpeed);
            var trafficTotal = new ObservablePoint(secondsToday, trafficTotalSpeeds);

            return (trafficIn, trafficOut, trafficTotal);
        }
        private void AddValuesToLineChart(double trafficInSpeed, double trafficOutSpeed, double trafficTotalSpeeds)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (trafficInSpeed > 0)
                {
                    var tuplePoints = CreateTrafficPointsTuple(trafficInSpeed, trafficOutSpeed, trafficTotalSpeeds);
                    TrafficInSpeed.Add(tuplePoints.TrafficIn);
                    TrafficOutSpeed.Add(tuplePoints.TrafficOut);
                    TrafficTotalSpeeds.Add(tuplePoints.TrafficTotal);
                    TrimChartExcessPoints();
                }
            });
        }



        private void TrimChartExcessPoints()
        {
            if (TrafficInSpeed.Count > 60)
            {
                TrafficInSpeed.RemoveAt(0);
                TrafficOutSpeed.RemoveAt(0);
                TrafficTotalSpeeds.RemoveAt(0);
            }
            
        }

        private void TrimRecord<T>( ObservableCollection<T> record)
        {
            if (record.Count > 60)
                record.RemoveAt(record.Count - 1);
        }

        private InterfaceDetails CreateInterfaceRecord(double trafficInVolume, double trafficOutVolume, double trafficInSpeed, double trafficOutSpeed, int interfaceSpeed)
        {
            InterfaceDetails interfaceDetails = new InterfaceDetails();
            interfaceDetails.Ifindex = Convert.ToInt16(IfIndex);
            interfaceDetails.DateTime = DateTime.Now.ToString("yyyy/MM/dd - HH:mm:ss");
            interfaceDetails.TrafficTotalVolume = Math.Round(trafficInVolume + trafficOutVolume, 3);
            interfaceDetails.TrafficInVolume = trafficInVolume;
            interfaceDetails.TrafficInSpeed = trafficInSpeed;
            interfaceDetails.TrafficOutSpeed = trafficOutSpeed;
            interfaceDetails.TrafficTotalSpeed = Math.Round(interfaceDetails.TrafficInSpeed + interfaceDetails.TrafficOutSpeed, 3);
            interfaceDetails.TrafficOutVolume = trafficOutVolume;
            interfaceDetails.InterfaceSpeed = interfaceSpeed;
            interfaceDetails.TrafficSpeedInColor = DetermineWarningColor(interfaceDetails.TrafficInSpeed, interfaceDetails.InterfaceSpeed);
            interfaceDetails.TrafficSpeedOutColor = DetermineWarningColor(interfaceDetails.TrafficOutSpeed, interfaceDetails.InterfaceSpeed);
            return interfaceDetails;
        }


        private SolidColorBrush DetermineWarningColor(double speed, int maxSpeed)
        {
            double mediumThreshold = maxSpeed * 0.50;
            double highThreshold = maxSpeed * 0.80;

            if (speed.IsBetween(mediumThreshold, highThreshold))
                return Brushes.Orange;
            else if (speed.IsLargest(highThreshold))
                return Brushes.Red;
            return Brushes.WhiteSmoke;
        }

        private void AddTrafficRecordToList(double trafficInVolume, double trafficOutVolume, double trafficInSpeed, double trafficOutSpeed, int interfaceSpeed)
        {
            InterfaceDetails interfaceDetails = CreateInterfaceRecord(trafficInVolume, trafficOutVolume, trafficInSpeed, trafficOutSpeed, interfaceSpeed);
            
            InterfaceDetails.Insert(0, interfaceDetails);
            TrimRecord<InterfaceDetails>(InterfaceDetails);
        }
        
    }
}
