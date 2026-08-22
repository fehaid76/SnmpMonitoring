using System;

namespace snmpMon.Services.AlertServices
{
    public interface IAlertNotify
    {
        event Action<string, string> OnAlertSent;
        void ShowErrors(string message);
        void ShowSuccess(string message);
        void ShowWrannings(string message);
    }
}