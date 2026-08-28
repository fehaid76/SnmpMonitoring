using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace snmpMon.Models
{
    public class InterfaceDetails
    {
        #region InterfaceInformation
        public string InterfaceName { get; set; }
        public int InterfaceSpeed {  get; set; }
        public int Ifindex { get; set; }
        #endregion

        #region TrafficTotal
        public double TrafficTotalSpeed { get; set; }
        public double TrafficTotalVolume { get; set; }
        #endregion

        #region TrafficIn
        public double TrafficInVolume { get; set; }
        public double TrafficInSpeed { get; set; }
        #endregion

        #region TrafficOut
        public double TrafficOutVolume { get; set; }
        public double TrafficOutSpeed { get; set; }
        #endregion

        #region DateTime
        public string DateTime { get; set; }
        #endregion

        #region SeverityColor
        public SolidColorBrush TrafficSpeedInColor {  get; set; }
        public SolidColorBrush TrafficSpeedOutColor { get; set; }
        #endregion


    }
}
