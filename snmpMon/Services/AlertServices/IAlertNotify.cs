namespace snmpMon.Services.AlertServices
{
    public interface IAlertNotify
    {
        void ShowErrors(string message);
        void ShowSuccess(string message);
        void ShowWrannings(string message);
    }
}