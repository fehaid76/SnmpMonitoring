using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Pipeline;
using snmpMon.Engine.Services.SnmpTraslatorServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace snmpMon.Engine.Services.SnmpTraslatorServices
{
    enum AlertLevels
    {
        Info = 1,
        Success,
        Warnning,
        Error
    }
    public class SnmpTranslator : ISnmpTranslator
    {
        
        private readonly Dictionary<string, Func<SnmpTrapPacket, SnmpAlert>> _trapHandlers;
        

        public SnmpTranslator()
        {
            _trapHandlers = new Dictionary<string, Func<SnmpTrapPacket, SnmpAlert>>
            {
                {"1.3.6.1.6.3.1.1.5.3", HandleLinkDown },
                {"1.3.6.1.4.1.9.9.41.2.0.1", HandleCiscoSyslog }
            };
        }



        public SnmpAlert ProcessIncomingTrap(SnmpTrapPacket trap)
        {
            if (_trapHandlers.TryGetValue(trap.TrapOid, out var handler))
            {
                return handler(trap);
            }

            return new SnmpAlert();
        }
        public SnmpTrapPacket GetSnmpTrapPacket(MessageReceivedEventArgs e)
        {
            SnmpTrapPacket snmpTrap = new SnmpTrapPacket();

            if (e.Message.Variables().Count > 0)
            {
                string _trapOid = e.Message.Pdu().ToString();
                snmpTrap.TrapOid = regexPDU(_trapOid);
                snmpTrap.VarBinds = new Dictionary<string, string>();
                foreach (var item in e.Message.Variables())
                {
                    string cleanKey = indexCut(item.Id.ToString());
                    snmpTrap.VarBinds[cleanKey] = item.Data.ToString();
                }
            }
            return snmpTrap;
            
        }

        private string regexPDU(string pdu)
        {
            string trapOid = string.Empty;
            string pattern = @"1\.3\.\d+(\.\d+)+";

            Match match = Regex.Match(pdu, pattern);

            if (match.Success)
            {
                trapOid = match.Value;
            }
            return trapOid;
        }
        private string indexCut(string index)
        {
            int lastDot = index.LastIndexOf('.');
            return index.Substring(0, lastDot);
        }
        private SnmpAlert HandleLinkDown(SnmpTrapPacket packet)
        {
            SnmpAlert snmpAlert = new SnmpAlert();
            packet.VarBinds.TryGetValue("1.3.6.1.2.1.2.2.1.2", out var varInterface);
            snmpAlert.AlertMessage = $"Interface {varInterface} status changed to down";
            snmpAlert.AlertLevel = (int)AlertLevels.Error;
            snmpAlert.AlertTime = DateTime.Now.ToString("HH:mm:ss");
            return snmpAlert;
            
        }

        private SnmpAlert HandleCiscoSyslog(SnmpTrapPacket packet)
        {
            SnmpAlert snmpAlert = new SnmpAlert();
            packet.VarBinds.TryGetValue("1.3.6.1.4.1.9.9.41.1.2.3.1.5", out var duplexMessage);
            string message = duplexMessage?.ToString() ?? "Unknown Syslog Event";
            snmpAlert.AlertMessage = message;
            snmpAlert.AlertLevel = checkForAlertLevel(message);
            snmpAlert.AlertTime = DateTime.Now.ToString("HH:mm:ss");
            return snmpAlert;
        }
        private int checkForAlertLevel(string msg)
        {
            if (msg.Contains("duplex mismatch"))
            {
                return (int)AlertLevels.Warnning;
            }
            else if (msg.Contains("up"))
            {
                return (int)AlertLevels.Success;
            }
            else if (msg.Contains("CONFIG_I") || msg.Contains("Configured"))
            {
                return (int)AlertLevels.Info;
            }

            return (int)AlertLevels.Info;
        }
    }
}
