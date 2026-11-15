using snmpMon.Engine.Helper.EventHandle;
using snmpMon.Engine.Services.SnmpTraslatorServices.Model;
using System;
using System.Threading.Tasks;

namespace snmpMon.Engine.Services.SnmpServices
{
    public interface ISnmpListener
    {
        event Action<string, SnmpAlert> OnSnmpTrapRecieved;
        public event EventHandler<InterfaceStatusEventArgs> OnInterfaceStatusChanged;

        void StartSnmpListener();
        void StopSnmpListener();
    }
}