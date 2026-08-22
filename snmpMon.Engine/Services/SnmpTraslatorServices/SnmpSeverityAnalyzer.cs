using Lextm.SharpSnmpLib;
using Newtonsoft.Json;
using snmpMon.Engine.Helper;
using snmpMon.Engine.Services.SnmpTraslatorServices.Enums;
using snmpMon.Engine.Services.SnmpTraslatorServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace snmpMon.Engine.Services.SnmpTraslatorServices
{

    public class SnmpSeverityAnalyzer : ISnmpSeverityAnalyzer
    {
        Dictionary<string, int> _severitykeywords;
        private HttpClient _httpClient;
        private IList<Variable> _snmpTraps;
        private string _trapPDu; 
        private CancellationTokenSource _cts;
        private readonly Task _initializationTask;
        public SnmpSeverityAnalyzer()
        {
            _cts = new CancellationTokenSource();
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("C# HttpClient/snmpMonAPP");

            _initializationTask = KeywordsMappingAsync();

        }

        public void FetchSnmpTrapVariablesAndPDU(IList<Variable> snmpTraps, string trapPDU)
        {
            if (snmpTraps != null && !string.IsNullOrWhiteSpace(trapPDU))
            {
                _snmpTraps = snmpTraps;
                _trapPDu = trapPDURegex(trapPDU);
             
            }
                
        }

        private string trapPDURegex(string trapPdu)
        {
            Match match = Regex.Match(trapPdu, @"enterprise:\s*([0-9.]+)");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return string.Empty;
        }
        
        private async Task<string> GetHttpDataAsync()
        {
            return await _httpClient.GetStringAsync("https://raw.githubusercontent.com/fehaid76/mib_json/refs/heads/main/Severity_levels.json");
        }

        private async Task<SnmpLogConfig?> DeserializeDataAsync()
        {
            var fetchedData = await GetHttpDataAsync();
            if (!string.IsNullOrEmpty(fetchedData))
            {
                var config = JsonConvert.DeserializeObject<SnmpLogConfig>(fetchedData);

                return config;
            }

            return new SnmpLogConfig();
        }

        public async Task StartSeverityLevelsMapping()
        {
            await KeywordsMappingAsync();

        }

        private async Task KeywordsMappingAsync()
        {
            if (_severitykeywords != null)
                return;

            var dataFethed = await DeserializeDataAsync();
            if (dataFethed?.NetworkEvents != null)
            {
                _severitykeywords = dataFethed.NetworkEvents.Where(x => x.Oid != null &&
                x.Severity.HasValue).GroupBy(x => x.Oid!, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(
                    g => g.Key,
                    g => g.First().Severity!.Value,
                    StringComparer.OrdinalIgnoreCase);

            }
        }

        private bool isSyslogEncapsulated()
        {
            return (_trapPDu == "1.3.6.1.4.1.9.9.41.2.0.1");
        }

        private int getEncapsulatedSeverityLevel()
        {
            if (_snmpTraps != null)
            {
                var sysMsg = _snmpTraps?.OrderByDescending(x => x.Data.ToString().Length).FirstOrDefault()?.ToString().ToLower();
                if(sysMsg.Contains("changed state to up"))
                {
                    return (int)AlertLevels.Notice;
                }
                else if (sysMsg.Contains("duplex"))
                {
                    return (int)AlertLevels.Error;
                }
                else if (sysMsg.Contains("cslu"))
                {
                    return (int)AlertLevels.Warning;
                }
                else if (sysMsg.Contains("too many fragments per datagram"))
                {
                    return (int)AlertLevels.Warning;
                }

            }

            return (int)AlertLevels.Info;

        }

        public int GetSeverityLevel()
        {
            _initializationTask.Wait();

            if (isSyslogEncapsulated())
            {
                return getEncapsulatedSeverityLevel();
            }
             
            int fetchedValue = 1;
            if (_severitykeywords == null)
                return fetchedValue;
            {
                
            }
            try
            {

                var severityLevel = _severitykeywords.TryGetValue(_trapPDu, out fetchedValue);
               
                //var matchedPair = _severitykeywords.Where(
                //    kvp => trapOID.StartsWith(kvp.Key, StringComparison.OrdinalIgnoreCase))
                //    .OrderByDescending(kvp => kvp.Key.Length).FirstOrDefault();

            }
            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
            }


            return fetchedValue;
        }


    }
}
