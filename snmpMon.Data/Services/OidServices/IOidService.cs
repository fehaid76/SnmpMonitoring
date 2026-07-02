namespace snmpMon.Engine.Services.OidServices
{
    public interface IOidService
    {
        string CpuUtilization();
        string FreeMemoeryOid();
        string UsedMemoeryOid();
        string SysUpTimeOid();
        string SysDetailsOid();
        string InterfacesDescOid();
        string InterfacesSpeed();
        string InteraceAdminStatusOid();
        string InterfaceOperStatusOid();
    }
}