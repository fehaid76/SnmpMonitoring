using snmpMon.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace snmpMon.Services.AlertServices
{
    public class AlertNotify : IAlertNotify
    {
        public event Action<string, string> OnAlertSent;
        private readonly Notifier _notifier;
        public AlertNotify()
        {
            _notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new PrimaryScreenPositionProvider(corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(4),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
        }

        public void ShowSuccess(string message)
        {
            _notifier.ShowSuccess(message);
            OnAlertSent?.Invoke(DateTime.Now.ToString("HH:mm:ss"), message);
            
        }

        public void ShowWrannings(string message)
        {
            _notifier.ShowWarning(message);
            OnAlertSent?.Invoke(DateTime.Now.ToString("HH:mm:ss"), message);

        }

        public void ShowErrors(string message)
        {
            _notifier.ShowError(message);
            OnAlertSent?.Invoke(DateTime.Now.ToString("HH:mm:ss"), message);

        }


    }
}
