using snmpMon.Engine.Helper;
using snmpMon.Engine.Helper.EventHandle;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.Helper.EventHandle;
using snmpMon.MibJson.Extenstions;
using snmpMon.MibJson.Services;
using snmpMon.MibParse.Models;
using snmpMon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace snmpMon.Services.DeviceServices
{
    public class NetworkInterfaceService : INetworkInterfaceService
    {
        public event Action<List<DeviceInterface>> OnDeviceInterfaceReceived;
        public event EventHandler<InterfaceStatusEventArgs> InterfaceStatusReceived;
        private string _ipAddress;
        private string _communityString;
        private List<DeviceInterface> _interfaces = new();
        private readonly IMibParser _mibParser;
        private readonly ISnmpService _snmpService;
        private readonly ISnmpListener _snmpListener;

        public NetworkInterfaceService(IMibParser mibParser, ISnmpService snmpService, ISnmpListener snmpListener)
        {
            _mibParser = mibParser;
            _snmpService = snmpService;
            _snmpListener = snmpListener;

            _snmpListener.OnInterfaceStatusChanged += _snmpListener_OnInterfaceStatusChanged;
        }

        private void _snmpListener_OnInterfaceStatusChanged(object sender, InterfaceStatusEventArgs e)
        {
            InterfaceStatusReceived?.Invoke(this, new InterfaceStatusEventArgs(e.IPAddress, e.IfDescr, e.IsUp));
        }

        private void PrepareDeviceInformation(string ipAddress, string communityString)
        {
            _ipAddress = ipAddress;
            _communityString = communityString;
        }

        private T GetParsedValue<T>(object value) where T : struct
        {
            if (value != null && Convert.ChangeType(value.ToString(), typeof(T)) is T result)
                return result;
            return default;

        }

        private void InterfacesListHandler((IList<SnmpVariable> Desc, IList<SnmpVariable> Speed, IList<SnmpVariable> Admin, IList<SnmpVariable> Oper) rawData)
        {
            _interfaces.Clear();

            _interfaces.AddRange(MapToDevicceInterfaces(rawData));

            OnDeviceInterfaceReceived?.Invoke(_interfaces);
        }

        private IEnumerable<DeviceInterface> MapToDevicceInterfaces((IList<SnmpVariable> Desc, IList<SnmpVariable> Speed, IList<SnmpVariable> Admin, IList<SnmpVariable> Oper) rawData)
        {
            var speedDic = rawData.Speed.ToDictionary(s => s.Oid.GetOidIndex(), s => GetParsedValue<long>(s.Data));
            var adminDic = rawData.Admin.ToDictionary(a => a.Oid.GetOidIndex(), a => GetParsedValue<int>(a.Data));
            var operDic = rawData.Oper.ToDictionary(o => o.Oid.GetOidIndex(), o => GetParsedValue<int>(o.Data));

            foreach (var desc in rawData.Desc)
            {
                var index = desc.Oid.GetOidIndex();

                yield return new DeviceInterface()
                {
                    Description = desc.Data.ToString(),
                    IfIndex = index,
                    Speed = speedDic.GetValueOrDefault(index),
                    AdminStatus = adminDic.GetValueOrDefault(index),
                    OperationStatus = operDic.GetValueOrDefault(index)
                };
            }
        }

        public async Task GetInterfacesAsync(string ipAddress, string communityString, CancellationTokenSource cancellationToken = null)
        {
            PrepareDeviceInformation(ipAddress, communityString);
            var oids = await fetchInterfaceOidsAsync();
            var rawResults = await fetchInterfaceDataSync(oids);
            InterfacesListHandler(rawResults);
        }

        private async Task<(IList<SnmpVariable> Desc, IList<SnmpVariable> Speed, IList<SnmpVariable> Admin, IList<SnmpVariable> Oper)>
            fetchInterfaceDataSync((MibNode Desc, MibNode Speed, MibNode Admin, MibNode Oper) Oids)
        {
            var descTask = _snmpService.GetBulkAsync(_ipAddress, _communityString, Oids.Desc.Oid);
            var speedTask = _snmpService.GetBulkAsync(_ipAddress, _communityString, Oids.Speed.Oid);
            var adminTask = _snmpService.GetBulkAsync(_ipAddress, _communityString, Oids.Admin.Oid);
            var operTask = _snmpService.GetBulkAsync(_ipAddress, _communityString, Oids.Oper.Oid);

            await Task.WhenAll(descTask, speedTask, adminTask, operTask);

            return (descTask.Result, speedTask.Result, adminTask.Result, operTask.Result);
        }

        private async Task<(MibNode Desc, MibNode Speed, MibNode Admin, MibNode Oper)> fetchInterfaceOidsAsync()
        {
            var descTask = _mibParser.GetNode("ifDescr");
            var speedTask = _mibParser.GetNode("ifSpeed");
            var adminTask = _mibParser.GetNode("ifAdminStatus");
            var operTask = _mibParser.GetNode("ifOperStatus");

            await Task.WhenAll(descTask, speedTask, adminTask, operTask);

            return (descTask.Result, speedTask.Result, adminTask.Result, operTask.Result);
        }
    }
}
