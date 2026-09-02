using snmpMon.ViewModels;
using System.Windows.Controls;

namespace snmpMon.Resources.Constrols.Frames.FrameServices
{
    public interface IFrameRequestServices
    {
       
        UserControl GetDevicesFrame();
        UserControl GetOverviewFrame();
        UserControl GetAlertsFrame();

    }
}