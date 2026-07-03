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

namespace snmpMon.Engine.Services.SnmpServices
{
    public class SnmpListener : ISnmpListener
    {
        public event Action<string, SnmpAlert> OnSnmpTrapRecieved;
        public event Action<string, bool> OnInterfaceStatusChanged;
        private string _cachedInterfaceIndex = string.Empty;
        private Listener _listener;
        private SnmpEngine _engine;
        private  UdpClient udpClient;
        private CancellationToken token;
        private readonly ISnmpTranslator _snmpTranslator;

        public SnmpListener(ISnmpTranslator snmpTranslator)
        {
            _snmpTranslator = snmpTranslator;
        }

        public void StartSnmpListener()
        {

            try
            {
                _listener = new Listener();
                var store = new ObjectStore();
                var factory = new SnmpApplicationFactory(store, null, null);
                var group = new EngineGroup();

                _engine = new SnmpEngine(factory, _listener, group);
                _listener.AddBinding(new IPEndPoint(IPAddress.Any, 162));




                _listener.MessageReceived += (sender, e) =>
                {

                    try
                    {
                        string message = string.Empty;
                        string routerIP = e.Sender.Address.ToString();

                        //Debug.WriteLine("###################[ Trap ]#######################");
                        //Debug.WriteLine(e.Message.Pdu());
                        //foreach (var item in e.Message.Variables())
                        //{
                        //    Debug.WriteLine(item);

                        //}
                        //Debug.WriteLine("##################################################");
                        var trapMessage = trapsAnalys(e);

                        OnSnmpTrapRecieved?.Invoke(routerIP, trapMessage);

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

                Task.Run(() =>
                {
                    try
                    {
                        _engine.Start();

                    }
                    catch (Exception ex)
                    {
                        Logger.LogCriticalException(ex, ex.Message);
                    }
                });


            }
            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);

            }
        }

        private void FitleringInterfacesTraps(Variable varBind)
        {
            var oid = varBind.Id.ToString();
            bool isUp = false;


            if (oid.StartsWith("1.3.6.1.4.1.9.9.138.1.3.3.1.3."))
            {
                _cachedInterfaceIndex = varBind.Data.ToString();
            }
            else if(oid.StartsWith("1.3.6.1.4.1.9.9.138.1.3.3.1.4."))
            {
                string statusValue = varBind.Data.ToString();
                isUp = statusValue != "1";
            }

            if (!string.IsNullOrEmpty(_cachedInterfaceIndex))
            {
                OnInterfaceStatusChanged?.Invoke(_cachedInterfaceIndex, isUp);
                _cachedInterfaceIndex = string.Empty;
            }

        }

        private SnmpAlert trapsAnalys(MessageReceivedEventArgs e)
        {
            var snmpTrapPacket = _snmpTranslator.GetSnmpTrapPacket(e);
            SnmpAlert trapMessage = new SnmpAlert();
            if (snmpTrapPacket != null)
            {
                trapMessage = _snmpTranslator.ProcessIncomingTrap(snmpTrapPacket);
            }

            return trapMessage;

        }
        public void StopSnmpListener()
        {
            
            _engine?.Stop();
        }

    }
}
