using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using snmpMon.Data.Model;
using snmpMon.Data.Repositories;
using snmpMon.Engine.Helper;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.Engine.Services.SnmpTraslatorServices.Enums;
using snmpMon.Engine.Services.SnmpTraslatorServices.Model;
using snmpMon.Extensions;
using snmpMon.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace snmpMon.ViewModel
{
    public class AlertViewModel : ObservableObject
    {
       
        private Dictionary<string, string> _devicesDetails = new Dictionary<string, string>();
        private ObservableCollection<SnmpAlertDetails> _alerts = new ObservableCollection<SnmpAlertDetails>();
        private string _routerIP;
        private int _routerID;
        private int _alertsCount;
        public int AlertsCount
        {
            get => _alertsCount;
            set
            {
                _alertsCount = value;
                OnPropertyChanged(nameof(AlertsCount));
            }
        }
        public bool IsGeneric = false; 
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
        public ObservableCollection<SnmpAlertDetails> Alerts
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
            _ = loadDevicesDetails();
            

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
                    arg2.IPAddress = arg1;
                    if (arg1 == RouterIP && !string.IsNullOrWhiteSpace(arg2.AlertMessage) && !IsGeneric)
                    {
                        Alerts.Insert(0, getSnmpAlertDetails(arg2));
                        
                    }
                    else if (!string.IsNullOrWhiteSpace(arg2.AlertMessage) && IsGeneric)
                    {
                        Alerts.Insert(0, getSnmpAlertDetails(arg2));
                        
                    }

                    checkForAlertsListCount();
                }

            });

        }
        private void checkForAlertsListCount()
        {
            if (Alerts.Count >= 100)
                Alerts.RemoveAt(Alerts.Count - 1);
        }
        private async Task loadDevicesDetails()
        {
            var devicesDet = await _deviceRepository.GetDeviceEndpointsAsync();
            _devicesDetails = devicesDet.ToDictionary(i => i.IPAddress, n => n.DeviceName);
        }

        private SnmpAlertDetails getSnmpAlertDetails(SnmpAlert snmpAlert)
        {
            var snmpAlertDet = new SnmpAlertDetails()
            {
                AlertMessage = snmpAlert.AlertMessage.SafeIntent(),
                AlertLevel = snmpAlert.AlertLevel,
                AlertTime = snmpAlert.AlertTime,
                AlertSeverity = getSeverity(snmpAlert.AlertLevel).SafeIntent(),
                IPAddress = snmpAlert.IPAddress.SafeIntent(),
                DeviceName = getDeviceName(snmpAlert.IPAddress).SafeIntent(),

            };

            return snmpAlertDet;
        }

        private string getDeviceName(string ipAddress)
        {
            if (!string.IsNullOrWhiteSpace(ipAddress))
            {
                if (_devicesDetails.TryGetValue(ipAddress, out var devName))
                {
                    return devName;
                }
            }

            return string.Empty;
        }
        private string getSeverity(int AlertLevel)
        {
            switch (AlertLevel)
            {
                case (int)AlertLevels.Emergency:
                    return "Emergency";
                case (int)AlertLevels.Alert:
                    return "Alert";
                case (int)AlertLevels.Critical:
                    return "Critical";
                case (int)AlertLevels.Error:
                    return "Error";
                case (int)AlertLevels.Warning:
                    return "Warning";
                case (int)AlertLevels.Notice:
                    return "Notice";
                case (int)AlertLevels.Info:
                    return "Info";
                case (int)AlertLevels.Debug:
                    return "Debug";

            }
            return "Notice";
        }


        private void Dispose()
        {
            _snmpListener.OnSnmpTrapRecieved -= _snmpListener_OnSnmpTrapRecieved;
            
        }
    }
}
