using Lextm.SharpSnmpLib.Pipeline;
using snmpMon.Engine.Services.SnmpTraslatorServices.Model;

namespace snmpMon.Engine.Services.SnmpTraslatorServices
{
    public interface ISnmpTranslator
    {
        SnmpAlert ProcessIncomingTrap(SnmpTrapPacket trap);
        SnmpTrapPacket GetSnmpTrapPacket(MessageReceivedEventArgs e);
    }
}