using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Engine.Services
{
    public static class ConnectionString
    {
        public static string connectionString() => "Server=.;Database=DBSNMP;User Id=sa;Password=123456; TrustServerCertificate=True;";
    }
}
