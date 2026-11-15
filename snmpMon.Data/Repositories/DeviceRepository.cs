using snmpMon.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using snmpMon.Data.Servicces;
using System.Diagnostics;
using snmpMon.Data.Extensions;

namespace snmpMon.Data.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        public async Task<bool> AddDeviceAsync(Device device)
        {
            int rowsAffected = 0;
            string insertQuery = @"INSERT INTO Devices(Name, Vendor,IPAddress, CommunityString, CategoryID)
                                    SELECT $Name, $Vendor, $IPAddress, $CommunityString, $CategoryID
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
                        cmd.Parameters.AddWithValue("$CategoryID", device.CategoryID);

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
                        cmd.Parameters.AddWithValue("$AlertTime", DateTime.Now.ToString("HH:mm:ss yyy:MM:dd"));

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
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            string query = @"SELECT * FROM Categories";
            List<Category> categories = new();
            using (var con = new SqliteConnection(ConnectionString.GetConnectionString()))
            {
                try
                {
                    await con.OpenAsync();
                    using (var cmd = new SqliteCommand(query, con))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync())
                                {
                                    Category category = new()
                                    {
                                        ID = Convert.ToInt32(reader["ID"]),
                                        Name = reader["Category"].ToString() ?? "N/A",
                                    };
                                    categories.Add(category);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                return categories;
            }
        }
        public async Task<List<Device>> GetAllDevicesAsync()
        {
            List<Device> _devices = new List<Device>();
            string sqlQuery = @"SELECT DeviceID, Name, Vendor, IPAddress, CommunityString, Category
                                FROM DevicesView;";
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
                                    d.ID = Convert.ToInt32(reader["DeviceID"]);
                                    d.Name = reader["Name"].ToString() ?? "N/A";
                                    d.IPAddress = reader["IPAddress"].ToString() ?? "N/A";
                                    d.Vendor = reader["Vendor"].ToString() ?? "N/A";
                                    d.CommunityString = reader["CommunityString"].ToString() ?? "N/A";
                                    d.Category = reader["Category"].ToString() ?? "N/A";
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
            string query = string.Empty;
            if (IPAddress == "0")
            {
                query = @"SELECT * FROM SnmpMessagesView";
            }
            else
            {
                query = @"SELECT * FROM SnmpMessagesView
                              WHERE IPAddress = $IPAddress;";
            }
             

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
                                    DeviceName = reader["DeviceName"].ToString().SafeIntern() ?? "N/A",
                                    IPAddress = reader["IPAddress"].ToString().SafeIntern() ?? "N/A",
                                    Message = reader["AlertMessage"].ToString().SafeIntern() ?? "N/A",
                                    Level = Convert.ToInt32(reader["AlertLevel"]),
                                    MessageTime = reader["AlertTime"].ToString()?.Replace(" ", " - ") ?? "N/A",
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
        public async Task<List<DeviceEndpoint>> GetDeviceEndpointsAsync()
        {
            List<DeviceEndpoint> _endPoints = new List<DeviceEndpoint>();
            string sqlQuery = @"SELECT ID, Name, IPAddress FROM Devices;";

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
                                    DeviceEndpoint endpoint = new DeviceEndpoint
                                    {
                                        ID = Convert.ToInt32(reader["ID"]),
                                        DeviceName = reader["Name"].ToString() ?? "N/A",
                                        IPAddress = reader["IPAddress"].ToString() ?? "N/A",
                                    };

                                    _endPoints.Add(endpoint);
                                }
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

                return _endPoints;
            }
        }
        public async Task<long> AddTemplatePathAsync(string Path)
        {
            string query = @"INSERT INTO Templates (TemplatePATH)
                                VALUES ($TemplatePath);
                                SELECT last_insert_rowid();";
            using (var con = new SqliteConnection(ConnectionString.GetConnectionString()))
            {
                try
                {
                    await con.OpenAsync();

                    using (var cmd = new SqliteCommand(query , con))
                    {
                        cmd.Parameters.AddWithValue("$TemplatePath", Path);

                        long newID = Convert.ToInt64(await cmd.ExecuteScalarAsync());
                        return newID;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                return 0;
            }
        }
        public async Task<bool> AddCategoryAsync(string Category, long ReleatedID)
        {
            string query = @"INSERT INTO Categories(Category, TemplateID)
	                           VALUES($Category, $TemplateID);";

            using(var con = new SqliteConnection(ConnectionString.GetConnectionString()))
            {
                try
                {
                    await con.OpenAsync();

                    using (var cmd = new SqliteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("$Category", Category);
                        cmd.Parameters.AddWithValue("$TemplateID", ReleatedID);

                        var rowsAf = await cmd.ExecuteNonQueryAsync();
                        return (rowsAf > 0);
                    }
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                return false;
            }
        }
        public async Task<List<string>> GetTemplatePathsAsync()
        {
            string query = @"SELECT TemplatePATH FROM Templates;";
            List<string> templates = new();
            using (var con = new SqliteConnection(ConnectionString.GetConnectionString()))
            {
                try
                {
                    await con.OpenAsync();
                    using (var cmd = new SqliteCommand(query, con))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync())
                                {
                                    templates.Add(reader["TemplatePATH"].ToString() ?? "N/A");
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                return templates;
            }
        }
        private void CratePragmaCommand(SqliteConnection connection)
        {
            using var pragmaCmd = connection.CreateCommand();
            pragmaCmd.CommandText = "PRAGMA foreign_keys = ON;";
            pragmaCmd.ExecuteNonQuery();
        }
        public async Task<bool> DeleteTemplateAsync(string categoryName)
        {
            string firstQuery = @"DELETE FROM Templates
                                  WHERE ID = ( SELECT TemplateID FROM Categories WHERE Category = $Category);";
            string seconQuery = @"DELETE FROM Categories
                                  WHERE Category = $Category;";
            bool isDeleted = false;
            int rowsDeleted = 0;
            using (var con = new SqliteConnection(ConnectionString.GetConnectionString()))
            {
                try
                {
                    await con.OpenAsync();
                    using (var transaction = con.BeginTransaction())
                    {
                        var command = con.CreateCommand();
                        command.Transaction = transaction;

                        try
                        {
                            command.CommandText = firstQuery;
                            command.Parameters.AddWithValue("$Category", categoryName);
                            rowsDeleted = await command.ExecuteNonQueryAsync();

                            command.Parameters.Clear();

                            command.CommandText = seconQuery;
                            command.Parameters.AddWithValue("$Category", categoryName);
                            rowsDeleted =  await command.ExecuteNonQueryAsync();

                            transaction.Commit();
                            isDeleted = true;

                        }
                        catch(Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            isDeleted = false;
                        }
                    }
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    isDeleted = false;
                }
                return isDeleted;
            }

        }
    }
}
