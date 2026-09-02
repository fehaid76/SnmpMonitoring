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
        public bool isConnected {  get; set; }
        public string SysDesc {  get; set; }
        public string Contact {  get; set; }
        [ObservableProperty]
        private string _systemName;
        public int SysSevices {  get; set; }
        [ObservableProperty]
        private string _location;
        //public string Location
        //{
        //    get => _location;
        //    set
        //    {
        //        _location = value;
        //        OnPropertyChanged(nameof(Location));
        //    }
        //}

        [ObservableProperty]
        private TimeSpan _upTime;
        //public TimeSpan UpTime
        //{
        //    get => _upTime;
        //    set
        //    {
        //        _upTime = value;
        //        OnPropertyChanged(nameof(UpTime));
        //    }
        //}
    }
}
