using snmpMon.Engine.Services.SnmpTraslatorServices.Model;
using System;
using System.Threading.Tasks;

namespace snmpMon.Engine.Services.SnmpServices
{
    public interface ISnmpListener
    {
        event Action<string, bool> OnInterfaceStatusChanged;
        event Action<string, SnmpAlert> OnSnmpTrapRecieved;

        void StartSnmpListener();
        void StopSnmpListener();
    }
}