using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using snmpMon.Data.Repositories;

namespace snmpMon.Engine.Services.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
        {
            services.AddTransient<IDeviceRepository, DeviceRepository>();

            return services;
        }
    }
}
