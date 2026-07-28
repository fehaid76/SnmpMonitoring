using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace snmpMon.ViewModel
{
    public enum MessageBoxIcon
    {
        Information = 0,
        Warning,
        Error,
        Question,

    }
    public class ConfirmDialogVM : ObservableObject
    {
        private MessageBoxIcon _icon;
        private string _title;
        private string _message;
        private bool _isOk;

        public MessageBoxIcon Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }
        public bool IsOk
        {
            get => _isOk;
            set
            {
                _isOk = value;
                OnPropertyChanged(nameof(IsOk));
            }
        }
        public ICommand CloseCommand { get; set; }
        public ICommand OkCommand { get; set; }
        public ConfirmDialogVM()
        {
            

            CloseCommand = new RelayCommand<Window>(w =>
            {
                w.Close();
            });

            OkCommand = new RelayCommand<Window>(w =>
            {
                IsOk = true;
                w.Close();
            });


        }
    }
}
