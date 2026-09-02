#pragma warning disable CA1416
using LiveChartsCore.Defaults;
using snmpMon.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace snmpMon.ViewModels.CardViewModels
{
    public partial class CardsViewModel
    {
        private void eventSubsecrib()
        {

            _snmpListener.OnSnmpTrapRecieved += HandleSnmpTraps;
            _overviewServices.OnAverageNetworkLatency += HandleAverageNetworkLatency;
            _overviewServices.OnUpDevices += HandleUpDevices;
            _devicesServices.OnDeviceDeleted += OnDeviceDeleted;
            _devicesServices.OnDeviceAdded += OnDeviceAdded;
            _overviewServices.OnInboundKbps += HandleInboundKbps;
            _overviewServices.OnTopInboundDevice += HandleTopInboundDevice;
            _overviewServices.OnOutboundKbps += HandleOutboundKbps;
        }

        private void HandleSnmpTraps(string arg1, Engine.Services.SnmpTraslatorServices.Model.SnmpAlert arg2)
        {
            if (!string.IsNullOrWhiteSpace(arg1) && arg2 != null)
            {
                AlertsCount++;
            }
        }

        private void HandleAverageNetworkLatency(double obj)
        {
            if (DevicesCount == 0)
                return;
            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                AverageLatency = obj;
                updateSeries(obj);


            });

        }

        private void HandleUpDevices(int obj)
        {
            DevicesCount = obj;
        }

        private void OnDeviceDeleted(string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                var tragetDevice = Devices.FirstOrDefault(d => d.IPAddress == obj);
                if (tragetDevice != null)
                {
                    Devices.Remove(tragetDevice);
                }
            }
        }

        private void OnDeviceAdded(Device obj)
        {
            if (obj != null)
                Devices.Add(obj);
        }

        private void clearLinerChart()
        {
            LatencyValues.Clear();
        }

        private void HandleInboundKbps(double obj)
        {


            Application.Current.Dispatcher?.Invoke(() =>
            {
                if (DevicesCount == 0)
                    return;

                double secondsToday = DateTime.Now.TimeOfDay.TotalSeconds;
                if (InTrafficValues.Count >= 30)
                {
                    var existingPoint = InTrafficValues[0];
                    existingPoint.X = secondsToday;
                    existingPoint.Y = obj;

                    InTrafficValues.RemoveAt(0);
                    InTrafficValues.Add(existingPoint);

                }
                else
                {
                    InTrafficValues.Add(new ObservablePoint(secondsToday, obj));
                }

            });

        }


        private void HandleTopInboundDevice(string obj)
        {
            TopConsumerDevice = obj;
        }

        private void HandleOutboundKbps(double obj)
        {
            if (DevicesCount == 0)
                return;

            Application.Current.Dispatcher?.Invoke(() =>
            {


                double secondsToday = DateTime.Now.TimeOfDay.TotalSeconds;
                if (OutTrafficValues.Count >= 30)
                {
                    var existingPoint = OutTrafficValues[0];
                    existingPoint.X = secondsToday;
                    existingPoint.Y = obj;

                    OutTrafficValues.RemoveAt(0);
                    OutTrafficValues.Add(existingPoint);

                }
                else
                {
                    OutTrafficValues.Add(new ObservablePoint(secondsToday, obj));
                }

            });
        }

        private void eventUnsubsecrib()
        {
            _snmpListener.OnSnmpTrapRecieved -= HandleSnmpTraps;
            _overviewServices.OnAverageNetworkLatency -= HandleAverageNetworkLatency;
            _overviewServices.OnUpDevices -= HandleUpDevices;
            _devicesServices.OnDeviceDeleted -= OnDeviceDeleted;
            _devicesServices.OnDeviceAdded -= OnDeviceAdded;
            _overviewServices.OnInboundKbps -= HandleInboundKbps;
            _overviewServices.OnTopInboundDevice -= HandleTopInboundDevice;
            _overviewServices.OnOutboundKbps -= HandleOutboundKbps;
        }
    }
}
