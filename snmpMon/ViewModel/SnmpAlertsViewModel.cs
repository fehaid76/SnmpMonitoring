using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using snmpMon.Data.Extensions;
using snmpMon.Data.Model;
using snmpMon.Engine.Services.SnmpTraslatorServices.Enums;
using snmpMon.Engine.Services.SnmpTraslatorServices.Model;
using snmpMon.Models;
using snmpMon.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace snmpMon.ViewModel
{
    public class SnmpAlertsViewModel : ObservableObject
    {
        private ObservableCollection<SnmpAlertDetails> _alerts = new ObservableCollection<SnmpAlertDetails>();
        public ObservableCollection<SnmpAlertDetails> Alerts
        {
            get => _alerts;
            set
            {
                _alerts = value;
                OnPropertyChanged(nameof(Alerts));
            }
        }
        public ICollectionView AlertsView { get; set; }

        private string _search; 
        public string Search
        {
            get => _search;
            set
            {
                _search = value;
                OnPropertyChanged(nameof(Search));
                filtering(value);
            }
        }

        public IAsyncRelayCommand RefreshCommand {  get; set; }
        private readonly IDevicesServices _devicesServices;
        public SnmpAlertsViewModel(IDevicesServices devicesServices)
        {
            _devicesServices = devicesServices;
            _ = _loadAllMessagesAsync();

            AlertsView = CollectionViewSource.GetDefaultView(Alerts);

            RefreshCommand = new AsyncRelayCommand(async () =>
            {
                await _refreshAlerts();
            });
        }

        private SnmpAlertDetails ConvertDeviceAlertToAlertDetails(DeviceAlert deviceAlert)
        {
            return new SnmpAlertDetails()
            {
                DeviceName = deviceAlert.DeviceName.SafeIntern(),
                AlertMessage = deviceAlert.Message.SafeIntern(),
                AlertLevel = deviceAlert.Level,
                AlertSeverity = getSeverity(deviceAlert.Level).SafeIntern(),
                AlertTime = deviceAlert.MessageTime,
                IPAddress = deviceAlert.IPAddress.SafeIntern(),
            };



        }

        private string getSeverity(int alertLevel)
        {
            switch (alertLevel)
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

        private async Task _refreshAlerts()
        {
            Alerts.Clear();
            await _loadAllMessagesAsync();
        }
        private void prepareSnmpAlertsToView(List<DeviceAlert> deviceAlerts)
        {

            if (deviceAlerts.Count > 0)
            {

                foreach (var msg in deviceAlerts)
                {
                    Alerts.Insert(0, ConvertDeviceAlertToAlertDetails(msg));
                }
            }

        }
        private async Task _loadAllMessagesAsync()
        {

            var messages = await _devicesServices.GetAllMessagesAsync();
            prepareSnmpAlertsToView(messages);

        }
        private void filtering(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                AlertsView.Filter = null;
            }
            else
            {
                AlertsView.Filter = item =>
                {
                    var alert = item as SnmpAlertDetails;
                    if (alert == null) return false;

                    return (alert.DeviceName != string.Empty && alert.DeviceName.ToLower().Contains(value.ToLower()) ||
                    alert.IPAddress != string.Empty && alert.IPAddress.Contains(value) || 
                    alert.AlertMessage != string.Empty && alert.AlertMessage.ToLower().Contains(value.ToLower()));
                };
            }

            AlertsView.Refresh();
        }
    }
}
