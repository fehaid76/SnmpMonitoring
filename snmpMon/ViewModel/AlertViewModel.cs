using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.Engine.Services.SnmpTraslatorServices.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.ViewModel
{
    public class AlertViewModel : ObservableObject
    {
        private ObservableCollection<SnmpAlert> _alerts = new ObservableCollection<SnmpAlert>();
        private string _routerIP;
        public string RouterIP
        {
            get => _routerIP;
            set
            {
                _routerIP = value;
                OnPropertyChanged(nameof(RouterIP));
            }
        }
        public ObservableCollection<SnmpAlert> Alerts
        {
            get => _alerts;
            set
            {
                _alerts = value;
                OnPropertyChanged(nameof(Alerts));
            }
        }

        public IRelayCommand CleanupCommand {  get; set; }

        private readonly ISnmpListener _snmpListener;
        public AlertViewModel(ISnmpListener snmpListener)
        {
            _snmpListener = snmpListener;
            _snmpListener.OnSnmpTrapRecieved += _snmpListener_OnSnmpTrapRecieved;
            CleanupCommand = new RelayCommand(() =>
            {
                Dispose();
            });
        }

        private void _snmpListener_OnSnmpTrapRecieved(string arg1, SnmpAlert arg2)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (arg2 != null)
                {
                    if(arg1 == RouterIP && !string.IsNullOrEmpty(arg2.AlertMessage))
                    {
                        Alerts.Insert(0, arg2);
                    }

                }
                    
            });

        }

        private void Dispose()
        {
            _snmpListener.OnSnmpTrapRecieved -= _snmpListener_OnSnmpTrapRecieved;
            
        }
    }
}
