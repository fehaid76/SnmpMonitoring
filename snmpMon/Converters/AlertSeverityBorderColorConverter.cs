using snmpMon.Engine.Services.SnmpTraslatorServices.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace snmpMon.Converters
{
    public class AlertSeverityBorderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null && int.TryParse(value.ToString(), out int severityLevel))
            {
                switch (severityLevel)
                {
                    case (int)AlertLevels.Emergency:
                        return (SolidColorBrush)new BrushConverter().ConvertFrom("#8B0000");
                    case (int)AlertLevels.Alert:
                        return (SolidColorBrush)new BrushConverter().ConvertFrom("#FF0000");
                    case (int)AlertLevels.Critical:
                        return (SolidColorBrush)new BrushConverter().ConvertFrom("#FF4500");
                    case (int)AlertLevels.Error:
                        return (SolidColorBrush)new BrushConverter().ConvertFrom("#FF8C00");
                    case (int)AlertLevels.Warning:
                        return (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD700");
                    case (int)AlertLevels.Notice:
                        return (SolidColorBrush)new BrushConverter().ConvertFrom("#1E90FF");
                    case (int)AlertLevels.Info:
                        return (SolidColorBrush)new BrushConverter().ConvertFrom("#00BFFF");
                    case (int)AlertLevels.Debug:
                        return (SolidColorBrush)new BrushConverter().ConvertFrom("#808080");

                }

                return (SolidColorBrush)new BrushConverter().ConvertFrom("#CCCCCC");
            }


            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
