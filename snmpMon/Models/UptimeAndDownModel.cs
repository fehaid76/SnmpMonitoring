using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Models
{
    public class UptimeAndDownModel : ObservableObject
    {
        private string _message;
        private string _messageTime;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }
        public string MessageTime
        {
            get => _messageTime;
            set
            {
                _messageTime = value;
                OnPropertyChanged(nameof(MessageTime));
            }
        }
    }
}
