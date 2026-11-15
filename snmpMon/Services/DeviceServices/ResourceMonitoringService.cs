using DynamicExpresso;
using Flee.PublicTypes;
using Lextm.SharpSnmpLib;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.Extensions;
using snmpMon.Helper.EventHandle;
using snmpMon.MibJson.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Services.DeviceServices
{
    public class ResourceMonitoringService : IResourceMonitoringService
    {
        public event EventHandler<ResourceUsageEventArgs> OnResourceDeviceUsage;
        private string _ipAddress;
        private string _communityString;
        private string _deviceCategory;

        private readonly ExpressionContext _context = new();
        private readonly DynamicExpresso.Interpreter _interpreter = new();

        private readonly ISnmpService _snmpService;
        private readonly IMibParser _mibParser;

        public ResourceMonitoringService(ISnmpService snmpService, IMibParser mibParser)
        {
            _mibParser = mibParser;
            _snmpService = snmpService;
        }

        private void PrepareDeviceInformation(string ipAddess, string communityString, string deviceCategory)
        {
            _ipAddress = ipAddess;
            _communityString = communityString;
            _deviceCategory = deviceCategory;
        }



        public (string MemoryUsage, string cpuTotal1Min) FetchUtilizationNodes(string ipAddress, string communityString, string deviceCategory)
        {
            PrepareDeviceInformation(ipAddress, communityString, deviceCategory);
            try
            {
                var deviceNode = _mibParser.GetDeviceNode(_deviceCategory);

                if (deviceNode?.Resources == null)
                {
                    throw new InvalidOperationException("Device node or resources configuration is null.");
                }

                var memoryOid = deviceNode.Resources.MemoryConfig?.MemoryUsageOID;
                var cpuOid = deviceNode.Resources.CpuUsage;

                return (
                    string.IsNullOrEmpty(memoryOid) ? string.Empty : memoryOid,
                    string.IsNullOrEmpty(cpuOid) ? string.Empty : cpuOid
                );
            }
            catch (Exception ex)
            {
                Engine.Helper.Logger.LogCriticalException(ex, ex.Message);
                return (string.Empty, string.Empty);
            }
        }

        public async Task GetResourceUsage(IPEndPoint ip, OctetString communityString, string MemoryUsageOid, string cpuUtilizationOid)
        {
            var memoryUsageTask = GetMemeoryUsageAsync(ip, communityString, MemoryUsageOid);
            var cpuUtilization = GetCPUtilizationAsync(ip, communityString, cpuUtilizationOid);

            await Task.WhenAll(memoryUsageTask, cpuUtilization);

            OnResourceDeviceUsage?.Invoke(this, new ResourceUsageEventArgs(memoryUsageTask.Result, cpuUtilization.Result));
        }

        private async Task<double> GetMemeoryUsageAsync(IPEndPoint ip, OctetString Community, string MemoryOid)
        {
            var memoryUsageDetails = await FetchMemoryUsage(ip, Community, MemoryOid);
            return Math.Round(memoryUsageDetails, 2);
        }

        private async Task<double> GetCPUtilizationAsync(IPEndPoint ip, OctetString Community, string cpuUtilizationOid)
        {
            Task<double> CpuTask = null;
            if (cpuUtilizationOid.GetOidsCount() > 1)
            {
                CpuTask = CalculateOidsString(cpuUtilizationOid, ip, Community);
            }
            else
            {
                CpuTask = _snmpService.GetDoubleAsync(ip, Community, new ObjectIdentifier(cpuUtilizationOid));
            }
            return await CpuTask;
        }

        private async Task<double> FetchMemoryUsage(IPEndPoint ip, OctetString Community, string MemoryOid)
        {
            Task<double> MemoryTask = null;

            if (MemoryOid.ToString().GetOidsCount() > 1)
            {
                MemoryTask = CalculateOidsString(MemoryOid.ToString(), ip, Community);
            }
            else
            {
                MemoryTask = _snmpService.GetDoubleAsync(ip, Community, new ObjectIdentifier(MemoryOid));

            }

            return await MemoryTask;

        }

        private async Task<double> CalculateOidsString(string value, IPEndPoint ip, OctetString Community)
        {
            var group = value.ConvertToOidList();
            var sd = new StringBuilder(value);
            var resultGroup = await _snmpService.GetGroupDoubleAsync(_ipAddress, _communityString, group);
            foreach (var o in resultGroup)
            {
                sd.Replace(o.oid, o.result.ToString());
            }
            return EvaluateFormal(sd.ToString());
        }

        private double EvaluateFormal(string formal)
        {
            var expression = formal.ReplaceSpaces().Trim('{', '}').AddDecimalToInt();
            var result = _interpreter.Eval(expression);


            return Math.Round(Convert.ToDouble(result), 2);
        }
    }
}
