using Lextm.SharpSnmpLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace snmpMon.Resources.Constrols.Frames.FrameServices
{
    public class FremeTransientRequestServices : IFremeTransientRequestServices
    {
        private readonly UserControl _snmpAlerts;

        public FremeTransientRequestServices()
        {
            _snmpAlerts = new SnmpAlerts();
        }

        public UserControl GetSNMPAlertsFrame()
        {
            return _snmpAlerts;
        }
    }
}
