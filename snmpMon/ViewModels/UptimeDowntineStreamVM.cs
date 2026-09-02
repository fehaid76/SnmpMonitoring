using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using snmpMon.Models;
using snmpMon.Services.AlertServices;
using snmpMon.Services.SensorServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.ViewModels
{
    public class UptimeDowntineStreamVM : ObservableObject
    {

        private ObservableCollection<UptimeAndDownModel> _devices = new ObservableCollection<UptimeAndDownModel>();
        public ObservableCollection<UptimeAndDownModel> Devices
        {
            get => _devices;
            set
            {
                _devices = value;
                OnPropertyChanged(nameof(Devices));
            }
        }


        private readonly IAlertNotify _alertNotify;
        public UptimeDowntineStreamVM(IAlertNotify alertNotify)
        {
            _alertNotify = alertNotify;
            _alertNotify.OnAlertSent += _alertNotify_OnAlertSent;
        }

        private void _alertNotify_OnAlertSent(string arg1, string arg2)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var alert = new UptimeAndDownModel()
                {
                    MessageTime = arg1,
                    Message = arg2
                };

                Devices.Insert(0, alert);
            });

        }
    }
}
