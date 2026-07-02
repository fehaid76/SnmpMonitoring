using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using snmpMon.Engine.Model;
using snmpMon.Engine.Services;
using System.Diagnostics;

namespace snmpMon.Engine.Data
{
    public class DeviceRepository : IDeviceRepository
    {
        public async Task AddDeviceAsync(DeviceData device)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString.connectionString()))
            {
                try
                {
                    string query = @"IF NOT EXISTS(SELECT 1 FROM Devices WHERE IPAddress = @IPAddress)
                                        BEGIN 
                                        INSERT INTO Devices (IPAddress, CommunityString)
                                        VALUES (@IPAddress, @CommunityString);
                                        END";

                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@IPAddress", device.IPAddress);
                        cmd.Parameters.AddWithValue("@CommunityString", device.CommunityString);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[Database add error]: {ex.Message}");
                }

            }
        }
        public async Task<List<DeviceData>> GetAllDevicesAsync()
        {
            List<DeviceData> devices = new List<DeviceData>();
            using (SqlConnection con = new SqlConnection(ConnectionString.connectionString()))
            {
                try
                {
                    string query = @"SELECT * FROM Devices;";

                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                DeviceData deviceData = new DeviceData
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    IPAddress = reader["IPAddress"].ToString(),
                                    CommunityString = reader["CommunityString"].ToString()
                                };
                                devices.Add(deviceData);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Debug.WriteLine($"[Read database error]: {ex.Message}");
                }
            }
            return devices;
        }
    }
}
