using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using snmpMon.Resources.Constrols.Devices;
using snmpMon.Resources.Constrols.Frames;
using snmpMon.Resources.Constrols.Frames.FrameServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace snmpMon.ViewModel
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

        public ICommand DevicesCmd { get; set; }
        public ICommand DashboardCmd { get; set; }

        private readonly IFrameRequestServices _requestServices;
        public MainViewModel(IFrameRequestServices requestServices)
        {
            _requestServices = requestServices;

            CurrentView = new Dashboard();

            DevicesCmd = new RelayCommand(() =>
            {
                CurrentView = _requestServices.GetDevicesFrame();
            });

            DashboardCmd = new RelayCommand(() =>
            {
                CurrentView = new Dashboard();
            });
            
        }
    }
}
