using CommunityToolkit.Mvvm.ComponentModel;
using snmpMon.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.ViewModel
{
    public class CardsViewModel :ObservableObject
    {
        private int _devicesCount;
        public int DevciesCount
        {
            get => _devicesCount;
            set
            {
                _devicesCount = value;
                OnPropertyChanged(nameof(DevciesCount));
            }
        }


        private readonly IDevicesServices _devicesServices;

        public CardsViewModel(IDevicesServices devicesServices)
        {
            _devicesServices = devicesServices;

            
        }


    }
}
