using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Data.Servicces
{
    public static class ConnectionString
    {
        
        public static string GetConnectionString() => "Data Source=snmpMonDB.db;Foreign Keys=True;";
    }
}
