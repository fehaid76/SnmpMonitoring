#pragma warning disable CA1416
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using snmpMon.Data.Model;
using snmpMon.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace snmpMon.ViewModels.CardViewModels
{
    public partial class CardsViewModel
    {
        [ObservableProperty]
        private ObservableCollection<SnmpAlertDetails> _alertDetails = new ObservableCollection<SnmpAlertDetails>();
        [ObservableProperty]
        private ObservableCollection<Axis> _xAxesList = new ObservableCollection<Axis>();
        [ObservableProperty]
        private ObservableCollection<ObservablePoint> _latencyValues = new ObservableCollection<ObservablePoint>();
        [ObservableProperty]
        private ObservableCollection<ObservablePoint> _inTrafficValues = new ObservableCollection<ObservablePoint>();
        [ObservableProperty]
        private ObservableCollection<ObservablePoint> _outTrafficValues = new ObservableCollection<ObservablePoint>();
        [ObservableProperty]
        public ObservableCollection<ISeries> _averageLatencyViews = new ObservableCollection<ISeries>();
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
    }
}
