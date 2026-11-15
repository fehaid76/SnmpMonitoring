using snmpMon.Engine.Helper;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Net.Sockets;
using snmpMon.Engine.Services.SnmpTraslatorServices;
using snmpMon.Engine.Services.SnmpTraslatorServices.Model;
using snmpMon.Data.Repositories;
using System.Text.RegularExpressions;
using snmpMon.Engine.Helper.EventHandle;

namespace snmpMon.Engine.Services.SnmpServices
{
    public class SnmpListener : ISnmpListener
    {
        public event Action<string, SnmpAlert> OnSnmpTrapRecieved;
        public event EventHandler<InterfaceStatusEventArgs> OnInterfaceStatusChanged;
        private string _ipAddress;
        private string _cachedInterfaceIndex = string.Empty;
        private Listener _listener;
        private SnmpEngine _engine;
        private  UdpClient udpClient;
        private CancellationToken token;
        private readonly ISnmpTranslator _snmpTranslator;
        private readonly IDeviceRepository _deviceRepository;
        public SnmpListener(ISnmpTranslator snmpTranslator, IDeviceRepository deviceRepository)
        {
            _snmpTranslator = snmpTranslator;
            _deviceRepository = deviceRepository;
        }

        private void StartListenerProcess()
        {
            if(_listener != null)
            {
                _listener.AddBinding(new IPEndPoint(IPAddress.Any, 162));
            }
        }

        private (ObjectStore Store, SnmpApplicationFactory Factory, EngineGroup Group) ConfigureSnmpEnvironment()
        {
            var store = new ObjectStore();
            var factory = new SnmpApplicationFactory(store, null, null);
            var group = new EngineGroup();

            return (store, factory, group);
        }



        private void HandleIncomingTrap()
        {
            if(_listener != null)
            {
                _listener.MessageReceived += (sender, e) =>
                {

                    try
                    {
                        string message = string.Empty;
                        string routerIP = e.Sender.Address.ToString();
                        _ipAddress = routerIP;
                        var trapMessage = trapsAnalys(e);
                        if (!string.IsNullOrWhiteSpace(trapMessage.AlertMessage))
                        {
                            OnSnmpTrapRecieved?.Invoke(routerIP, trapMessage);
                            _ = uploadMessagesToDatabase(routerIP, trapMessage);
                        }

                    }
                    catch (System.Net.Sockets.SocketException ex)
                    {
                        Logger.LogCriticalException(ex, "Port 162 is used by another application!");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogCriticalException(ex, ex.Message);
                    }

                };

            }
        }
        private void StartEngineProcess()
        {
            Task.Run(() =>
            {
                try
                {
                    _engine.Start();
                }
                catch(Exception ex)
                {
                    Logger.LogCriticalException(ex, ex.Message);
                }
            });
        }
        public void StartSnmpListener()
        {

            try
            {
                _listener = new Listener();

                var snmpEngineFactory = ConfigureSnmpEnvironment();
                _engine = new SnmpEngine(snmpEngineFactory.Factory, _listener, snmpEngineFactory.Group);

                StartListenerProcess();
                HandleIncomingTrap();
                StartEngineProcess();

            }
            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);

            }
        }

        private async Task uploadMessagesToDatabase(string ipAddress, SnmpAlert snmpAlert)
        {
            if (!string.IsNullOrEmpty(ipAddress) && snmpAlert != null)
            {
                var isUploaded = await _deviceRepository.AddAlertMessageAsync(snmpAlert.AlertMessage,
                                 snmpAlert.AlertLevel, snmpAlert.AlertTime, ipAddress);
            }


        }

        private SnmpAlert trapsAnalys(MessageReceivedEventArgs e)
        {
            var snmpTrapPacket = _snmpTranslator.GetSnmpTrapPacket(e);
            SnmpAlert trapMessage = new SnmpAlert();
            if (snmpTrapPacket != null)
            {
                trapMessage = _snmpTranslator.ProcessIncomingTrap(snmpTrapPacket);
                CheckForInterfaceStatusChanged(trapMessage.AlertMessage);
            }

            return trapMessage;

        }

        private void CheckForInterfaceStatusChanged(string trapMessage)
        {
            if (string.IsNullOrEmpty(trapMessage))
                return;
            string interfaceName = string.Empty;
            string pattern = @"Interface\s+([A-Za-z0-9/\.]+);?";
            bool isUp = false;
            if (trapMessage.Contains("Interface"))
            {
                Match match = Regex.Match(trapMessage, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    interfaceName = match.Groups[1].Value.TrimEnd(',');
                    isUp = trapMessage.ToLower().Contains("up");
                    OnInterfaceStatusChanged?.Invoke(this, new InterfaceStatusEventArgs(_ipAddress, interfaceName, isUp));

                }

            }   
        }

        public void StopSnmpListener()
        {
            
            _engine?.Stop();
        }

    }
}
