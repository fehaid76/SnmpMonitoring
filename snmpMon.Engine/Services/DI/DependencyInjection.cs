using Microsoft.Extensions.DependencyInjection;
using snmpMon.Data.Repositories;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.Engine.Services.SnmpTraslatorServices;
using snmpMon.MibJson.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Engine.Services.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
        {
            services.AddTransient<IDeviceRepository, DeviceRepository>();
            services.AddTransient<ISnmpService, SnmpService>();
            services.AddSingleton<ISnmpSeverityAnalyzer, SnmpSeverityAnalyzer>();
            services.AddSingleton<IMibParser, MibParser>();
            return services;
        }
    }
}
