using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace snmpMon.Engine.Services.SnmpTraslatorServices.Model
{
    public class SnmpAlert
    {
        public string AlertMessage {  get; set; }
        public int AlertLevel {  get; set; }
        public string AlertTime {  get; set; }

        public string WarningLevelIcon
        {
            get
            {
                switch (AlertLevel)
                {
                    case (int)AlertLevels.Success:
                        return "/Resources/Imgs/AlertImgs/Success.png";
                    case (int)AlertLevels.Warnning:
                        return "/Resources/Imgs/AlertImgs/Warnning.png";
                    case (int)AlertLevels.Error:
                        return "/Resources/Imgs/AlertImgs/Error.png";
                    case (int)AlertLevels.Info:
                        return "/Resources/Imgs/AlertImgs/Info.png";
                }
                return "";
            }
        }
    }
}
