using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using snmpMon.Core.Services;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.Enums;
using snmpMon.MibJson.Services;
using snmpMon.MibParse.Models;
using snmpMon.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace snmpMon.ViewModel
{
    public class VerificationViewModel : ObservableObject
    {
        private DeviceVerification _deviceVerification = new DeviceVerification();
        private string _ipAddress;
        private string _communityString;

        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                _ipAddress = value;
                OnPropertyChanged(nameof(IpAddress));
            }
        }

        public string CommunityString
        {
            get => _communityString;
            set
            {
                _communityString = value;
                OnPropertyChanged(nameof(CommunityString));
            }
        }

        public DeviceVerification DeviceVerification
        {
            get => _deviceVerification;
            set
            {
                _deviceVerification = value;
                OnPropertyChanged(nameof(DeviceVerification));
            }
        }

        public ICommand VerificationCommand {  get; set; }

        private readonly IDeviceVerficationServices _deviceVerficationServices;
        private readonly IMibParser _mibParser;
        public VerificationViewModel(IDeviceVerficationServices deviceVerficationServices, IMibParser mibParser)
        {
            _deviceVerficationServices = deviceVerficationServices;
            _mibParser = mibParser;

            getDefualtVerificationStatus();
            eventsSubsecribe();
            VerificationCommand = new RelayCommand(async () =>
            {
                await StartVerificationProccess(IpAddress, CommunityString);
            });
        }

        private void getDefualtVerificationStatus()
        {
            DeviceVerification.PingVerification = VarificationStatus.Idle;
            DeviceVerification.SNMPProtocolVerification = VarificationStatus.Idle;
            DeviceVerification.DeviceVendorVerification = VarificationStatus.Idle;
        }
        private void eventsSubsecribe()
        {
            _deviceVerficationServices.OnSnmpProtocolSuccess += _deviceVerficationServices_OnSnmpProtocolSuccess;
            _deviceVerficationServices.OnPingSuccess += _deviceVerficationServices_OnPingSuccess;
            _deviceVerficationServices.OnDeviceVendorRecieved += _deviceVerficationServices_OnDeviceVendorRecieved;
        }

        private void _deviceVerficationServices_OnDeviceVendorRecieved(string obj)
        {
            if(!string.IsNullOrEmpty(obj))
                DeviceVerification.DeviceVendorVerification = ConvertVerificationStatusToEnum(true);
            else
            {
                DeviceVerification.DeviceVendorVerification = ConvertVerificationStatusToEnum(false);

            }
        }

        private void _deviceVerficationServices_OnPingSuccess(bool obj)
        {
            DeviceVerification.PingVerification = ConvertVerificationStatusToEnum(obj);
        }

        private void _deviceVerficationServices_OnSnmpProtocolSuccess(bool obj)
        {
            DeviceVerification.SNMPProtocolVerification = ConvertVerificationStatusToEnum(obj);
        }

        private VarificationStatus ConvertVerificationStatusToEnum(bool status)
        {
            if (status)
                return VarificationStatus.Success;
            return VarificationStatus.Failure;
        }
        private async Task StartVerificationProccess(string ipAddress, string communityString)
        {
            IconsToWaitStatus();
            var node = GetMibNode();
            await _deviceVerficationServices.StartVerificationDevice(ipAddress, communityString, node);
        }

        private MibNode GetMibNode()
        {
            return _mibParser.GetNode("sysObjectID");
        }

        private void IconsToWaitStatus()
        {
            DeviceVerification.PingVerification = VarificationStatus.Waiting;
            DeviceVerification.SNMPProtocolVerification = VarificationStatus.Waiting;
            DeviceVerification.DeviceVendorVerification = VarificationStatus.Waiting;
        }
        
    }
}
