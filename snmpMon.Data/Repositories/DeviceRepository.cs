using snmpMon.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using snmpMon.Data.Servicces;
using System.Diagnostics;

namespace snmpMon.Data.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        public async Task<bool> AddDeviceAsync(Device device)
        {
            int rowsAffected = 0;
            string insertQuery = @"INSERT INTO Devices(Name, Vendor,IPAddress, CommunityString)
                                    SELECT $Name, $Vendor, $IPAddress, $CommunityString
                                    WHERE NOT EXISTS(
	                                    SELECT 1 FROM Devices WHERE IPAddress = $IPAddress);";

            using (var con = new SqliteConnection(ConnectionString.GetConnectionString()))
            {
                try
                {
                    await con.OpenAsync();
                    

                    using (var cmd = new SqliteCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("$Name", device.Name ?? "");
                        cmd.Parameters.AddWithValue("$Vendor", device.Vendor ?? "");
                        cmd.Parameters.AddWithValue("$IPAddress", device.IPAddress);
                        cmd.Parameters.AddWithValue("$CommunityString", device.CommunityString);

                        rowsAffected = await cmd.ExecuteNonQueryAsync();

                    }
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

            }


            return (rowsAffected > 0);
        }
        public async Task<bool> AddAlertMessageAsync(string AlertMessage, int AlertLevel, string AlertTime, string IPAddress)
        {
            int rowsAf = 0;
            string query = @"INSERT INTO SnmpMessages(DeviceID, AlertMessage, AlertLevel, AlertTime)
                                VALUES(
                                (SELECT ID 
	                                FROM Devices
	                                WHERE IPAddress = $IPAddress),
                                $AlertMessage,
                                $AlertLevel,
                                $AlertTime);";

            using (var con = new SqliteConnection(ConnectionString.GetConnectionString()))
            {
                try
                {
                    await con.OpenAsync();

                    using (var cmd = new SqliteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("$IPAddress", IPAddress);
                        cmd.Parameters.AddWithValue("$AlertMessage", AlertMessage);
                        cmd.Parameters.AddWithValue("$AlertLevel", AlertLevel);
                        cmd.Parameters.AddWithValue("$AlertTime", AlertTime);

                        rowsAf = await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return rowsAf > 0;
        }

        public async Task<List<Device>> GetAllDevicesAsync()
        {
            List<Device> _devices = new List<Device>();
            string sqlQuery = @"SELECT ID, Name, Vendor, IPAddress, CommunityString
                              FROM Devices;";
            using (var con = new SqliteConnection(ConnectionString.GetConnectionString()))
            {
                try
                {
                    await con.OpenAsync();

                    using (var cmd = new SqliteCommand(sqlQuery, con))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                
                                while (await reader.ReadAsync())
                                {
                                    Device d = new Device();
                                    d.ID = Convert.ToInt32(reader["ID"]);
                                    d.Name = reader["Name"].ToString() ?? "N/A";
                                    d.IPAddress = reader["IPAddress"].ToString() ?? "N/A";
                                    d.Vendor = reader["Vendor"].ToString() ?? "N/A";
                                    d.CommunityString = reader["CommunityString"].ToString() ?? "N/A";

                                    _devices.Add(d);

                                }
                            }
                        }
                    }
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return _devices;
        }
        public async Task<List<DeviceAlert>> GetAllMessagesAsync(string IPAddress)
        {
            List<DeviceAlert> _deviceAlerts = new List<DeviceAlert>();
            string query = @"SELECT * FROM SnmpMessagesView
                              WHERE IPAddress = $IPAddress;";

            using (var con = new SqliteConnection(ConnectionString.GetConnectionString()))
            {
                try
                {
                    await con.OpenAsync();
                    using (var cmd = new SqliteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("$IPAddress", IPAddress);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                DeviceAlert d = new DeviceAlert
                                {
                                    IPAddress = reader["IPAddress"].ToString() ?? "N/A",
                                    Message = reader["AlertMessage"].ToString() ?? "N/A",
                                    Level = Convert.ToInt32(reader["AlertLevel"]),
                                    MessageTime = reader["AlertTime"].ToString() ?? "N/A",
                                };

                                _deviceAlerts.Add(d);
                            }
                        }
                    }
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                return _deviceAlerts;
            }

        }
        public async Task<bool> DeleteDeviceAsync(string ipAddress)
        {
            int rowsAf = 0;
            string query = @"DELETE FROM Devices WHERE IPAddress = $IPAddress";

            using (var con = new SqliteConnection(ConnectionString.GetConnectionString()))
            {
                try
                {
                    await con.OpenAsync();

                    using (var cmd = new SqliteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("$IPAddress", ipAddress);

                        rowsAf = await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return rowsAf > 0;
        }
    }
}
