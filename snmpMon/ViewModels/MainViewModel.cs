using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using snmpMon.Data.Model;
using snmpMon.Resources.Constrols.Devices;
using snmpMon.Resources.Constrols.Frames;
using snmpMon.Resources.Constrols.Frames.FrameServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace snmpMon.ViewModels
{
    public  class MainViewModel : ObservableObject
    {
        private object _currentView; 
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        private readonly object _dashboardView;
        private object _devicesView; 
        public ICommand DevicesCmd { get; set; }
        public ICommand DashboardCmd { get; set; }
        public ICommand SnmpAlerts {  get; set; }

        private readonly IFrameRequestServices _requestServices;
        private readonly IFremeTransientRequestServices _fremeTransientRequest;
        public MainViewModel(IFrameRequestServices requestServices, IFremeTransientRequestServices fremeTransientRequest)
        {
            _requestServices = requestServices;
            _fremeTransientRequest = fremeTransientRequest;
           
            _dashboardView = _requestServices.GetOverviewFrame();
            CurrentView = _dashboardView;

            DevicesCmd = new RelayCommand(() =>
            {
                _devicesView = _requestServices.GetDevicesFrame();
                CurrentView = _devicesView;

            });

            DashboardCmd = new RelayCommand(() =>
            {
                CurrentView = _dashboardView;
            });

            SnmpAlerts = new RelayCommand(() =>
            {
                CurrentView = _fremeTransientRequest.GetSNMPAlertsFrame();
            });

        }

    }
}
