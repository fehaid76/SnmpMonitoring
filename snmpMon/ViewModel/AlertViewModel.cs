using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using snmpMon.Data.Repositories;
using snmpMon.Engine.Helper;
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
        private int _routerID;

        public int RouterID
        {
            get => _routerID;
            set
            {
                _routerID = value;
                OnPropertyChanged(nameof(RouterID));
            }
        }
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
        private readonly IDeviceRepository _deviceRepository;
        public AlertViewModel(ISnmpListener snmpListener, IDeviceRepository deviceRepository)
        {
            _snmpListener = snmpListener;
            _deviceRepository = deviceRepository;

            _snmpListener.OnSnmpTrapRecieved += _snmpListener_OnSnmpTrapRecieved;

            _= _getAllMessages();

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
        private async Task _getAllMessages()
        {
            try
            {

                if (Alerts.Count < 1)
                {
                    await Task.Delay(500);
                    var msgs = await _deviceRepository.GetAllMessagesAsync(RouterIP);

                    await App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        foreach (var msg in msgs)
                        {
                            SnmpAlert alert = new SnmpAlert
                            {
                                AlertMessage = msg.Message,
                                AlertLevel = msg.Level,
                                AlertTime = msg.MessageTime,
                            };
                            Alerts.Insert(0, alert);
                        }
                    });
                }

            }
            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
            }
        }
        private void Dispose()
        {
            _snmpListener.OnSnmpTrapRecieved -= _snmpListener_OnSnmpTrapRecieved;
            
        }
    }
}
