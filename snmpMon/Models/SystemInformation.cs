using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Models
{
    public partial class SystemInformation : ObservableObject
    {
        // public bool isConnected {  get; set; }
        [ObservableProperty]
        private bool _isConnected;
        public string SysDesc {  get; set; }
        public string Contact {  get; set; }
        [ObservableProperty]
        private string _systemName;
        public int SysSevices {  get; set; }
        [ObservableProperty]
        private string _location;
        [ObservableProperty]
        private TimeSpan _upTime;

    }
}
