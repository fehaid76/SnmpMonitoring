using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Data.Servicces
{
    public static class ConnectionString
    {
        
        //public static string GetConnectionString() => "Data Source=snmpMonDB.db;Foreign Keys=True;";

        public static string GetConnectionString()
        {
            string appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "snmpMon");

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            string dpPath = Path.Combine(appDataFolder, "snmpMonDB.db");

            return $"Data Source={dpPath};Foreign Keys=True;";
        }
    }
}
