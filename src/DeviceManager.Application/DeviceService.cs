using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using DeviceManager.Application.dtos;
using Devices.devices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DeviceManager.Application;

public class DeviceService : IDeviceService
{
    private string _connectionString;

    public DeviceService(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public IEnumerable<DeviceDTO> GetAllDevices()
    {
        List<DeviceDTO> devices = [];
        const string query = "SELECT * FROM Device";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var deviceRow = new DeviceDTO
                        {
                            Id = reader.GetString(0),
                            Name = reader.GetString(1),
                            IsOn = reader.GetBoolean(2)
                        };
                        devices.Add(deviceRow);
                    }
                }
            }
            finally
            {
                reader.Close();
            }
            return devices;
        }
    }

    public Device GetDeviceById(string id)
    {
        var query = "SELECT * FROM Device";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            if (id.Contains("SW"))
            {
                query += " JOIN SmartWatch ON Device.Id = SmartWatch.Device_id WHERE SmartWatch.Device_Id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    if (reader.Read())
                    {
                        return new SmartWatch
                        {
                            Id = reader.GetString(0),
                            Name = reader.GetString(1),
                            IsOn = reader.GetBoolean(2),
                            BatteryCharge = reader.GetInt32(4)
                        };
                    }
                }
                finally
                {
                    reader.Close();
                }
            } else if (id.Contains("P"))
            {
                query += " JOIN PersonalComputer ON Device.Id = PersonalComputer.Device_id WHERE PersonalComputer.Device_Id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    if (reader.Read())
                    {
                        return new PersonalComputer
                        {
                            Id = reader.GetString(0),
                            Name = reader.GetString(1),
                            IsOn = reader.GetBoolean(2),
                            OperatingSystem = reader.GetString(4)
                        };
                    }
                }
                finally
                {
                    reader.Close();
                }
            } else if (id.Contains("ED"))
            {
                query += " JOIN EmbeddedDevice on Device.Id = EmbeddedDevice.Device_id WHERE EmbeddedDevice.Device_id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    if (reader.Read())
                    {
                        return new EmbeddedDevice
                        {
                            Id = reader.GetString(0),
                            Name = reader.GetString(1),
                            IsOn = reader.GetBoolean(2),
                            IpAddress = reader.GetString(4),
                            NetworkName = reader.GetString(5)
                        };
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        return null;
    }

    // public bool AddDevice(Device device)
    // {
    //     var countDeviceRowsAdded = -1;
    //     var countSpeficifDeviceRowsAdded = -1;
    //     using (SqlConnection connection = new SqlConnection(_connectionString))
    //     {
    //         if (device.Id.Contains("SW"))
    //         {
    //             var smartWatch = (SmartWatch) device;
    //             const string deviceQuery = "INSERT INTO Device VALUES (@Id, @Name, @IsOn)";
    //             var smartWatchQuery = "INSERT INTO SmartWatch VALUES (@BatteryCharge, @Device_id)";
    //             
    //             SqlCommand deviceCommand = new SqlCommand(deviceQuery, connection);
    //             deviceCommand.Parameters.AddWithValue("@Id", device.Id);
    //             deviceCommand.Parameters.AddWithValue("@Name", device.Name);
    //             deviceCommand.Parameters.AddWithValue("@IsOn", device.IsOn);
    //             
    //             SqlCommand smartWatchCommand = new SqlCommand(smartWatchQuery, connection);
    //             smartWatchCommand.Parameters.AddWithValue("@BatteryCharge", smartWatch.BatteryCharge);
    //             smartWatchCommand.Parameters.AddWithValue("@Device_id", device.Id);
    //             
    //             connection.Open();
    //
    //             countDeviceRowsAdded = deviceCommand.ExecuteNonQuery();
    //             countSpeficifDeviceRowsAdded = smartWatchCommand.ExecuteNonQuery();
    //         }
    //         if (device.Id.Contains("P"))
    //         {
    //             var personalComputer = (PersonalComputer) device;
    //             const string deviceQuery = "INSERT INTO Device VALUES (@Id, @Name, @IsOn)";
    //             var smartWatchQuery = "INSERT INTO PersonalComputer VALUES (@OperatingSystem, @Device_id)";
    //             
    //             SqlCommand deviceCommand = new SqlCommand(deviceQuery, connection);
    //             deviceCommand.Parameters.AddWithValue("@Id", device.Id);
    //             deviceCommand.Parameters.AddWithValue("@Name", device.Name);
    //             deviceCommand.Parameters.AddWithValue("@IsOn", device.IsOn);
    //             
    //             SqlCommand personalComputerCommand = new SqlCommand(smartWatchQuery, connection);
    //             personalComputerCommand.Parameters.AddWithValue("@OperatingSystem", personalComputer.OperatingSystem);
    //             personalComputerCommand.Parameters.AddWithValue("@Device_id", device.Id);
    //             
    //             connection.Open();
    //
    //             countDeviceRowsAdded = deviceCommand.ExecuteNonQuery();
    //             countSpeficifDeviceRowsAdded = personalComputerCommand.ExecuteNonQuery();
    //         }
    //         if (device.Id.Contains("ED"))
    //         {
    //             var embeddedDevice = (EmbeddedDevice) device;
    //             const string deviceQuery = "INSERT INTO Device VALUES (@Id, @Name, @IsOn)";
    //             var smartWatchQuery = "INSERT INTO EmbeddedDevice VALUES (@IpAddress, @NetworkName, @IsConnected, @Device_id)";
    //             
    //             SqlCommand deviceCommand = new SqlCommand(deviceQuery, connection);
    //             deviceCommand.Parameters.AddWithValue("@Id", device.Id);
    //             deviceCommand.Parameters.AddWithValue("@Name", device.Name);
    //             deviceCommand.Parameters.AddWithValue("@IsOn", device.IsOn);
    //             
    //             SqlCommand embeddedDeviceCommand = new SqlCommand(smartWatchQuery, connection);
    //             embeddedDeviceCommand.Parameters.AddWithValue("@IpAddress", embeddedDevice.IpAddress);
    //             embeddedDeviceCommand.Parameters.AddWithValue("@NetworkName", embeddedDevice.NetworkName);
    //             embeddedDeviceCommand.Parameters.AddWithValue("@Device_id", device.Id);
    //             embeddedDeviceCommand.Parameters.AddWithValue("@Device_id", device.Id);
    //             
    //             connection.Open();
    //
    //             countDeviceRowsAdded = deviceCommand.ExecuteNonQuery();
    //             countSpeficifDeviceRowsAdded = embeddedDeviceCommand.ExecuteNonQuery();
    //         }
    //     }
    //     return countDeviceRowsAdded != -1 && countSpeficifDeviceRowsAdded != -1;
    // }

    public bool AddDeviceByJson(JsonNode? json)
    {
        var deviceType = json["deviceType"]?.ToString();
        if (string.IsNullOrEmpty(deviceType))
        {
            throw new ArgumentException("Invalid JSON format. deviceType is not specified.");
        }
        
        switch (deviceType.ToLower())
        {
            case "smartwatch":
            {
                AddSmartWatch(json);
                break;
            }
            
            case "personalcomputer":
            {
                AddPersonalComputer(json);
                break;
            }
            
            case "embeddeddevice":
            {
                AddEmbeddedDevice(json);
                break;
            }

            default:
                throw new ArgumentException("Uknown device type.");
        }
        
        return false;
    }
    
    private void AddSmartWatch(JsonNode? json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        SmartWatch? smartWatch;
        try
        {
            smartWatch = JsonSerializer.Deserialize<SmartWatch>(json, options);
        }
        catch
        {
            throw new ArgumentException("JSON deserialization failed. Seek help.");
        }
        
        if (smartWatch == null)
            throw new ArgumentException("JSON deserialization failed. Seek help.");
        
        // edgecases
        if (smartWatch.BatteryCharge is < 0 or > 100)
            throw new ArgumentException("JSON deserialization failed. Battery charge is out of range [0 - 100].");
        
        // adding id and inserting into the db
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            var countSwQuery = "SELECT COUNT(*) FROM SmartWatch";
            var count = -1;
            
            SqlCommand countCommand = new SqlCommand(countSwQuery, connection);
            connection.Open();
            SqlDataReader reader = countCommand.ExecuteReader();
            try
            {
                if (reader.Read())
                {
                    count = reader.GetInt32(0);
                }
            }
            finally
            {
                reader.Close();
            }
            smartWatch.Id = $"SW-{count + 1}";

            var insertDeviceResult = -1;
            var insertWatchResult = -1;

            var insertDeviceQuery = $"INSERT INTO Device VALUES (@Id, @Name, @IsOn)";
            var insertWatchQuery = $"INSERT INTO SmartWatch VALUES (@Id, @BatteryCharge, @Device_id)";
            SqlCommand insertDeviceCommand = new SqlCommand(insertDeviceQuery, connection);
            insertDeviceCommand.Parameters.AddWithValue("@Id", smartWatch.Id);
            insertDeviceCommand.Parameters.AddWithValue("@Name", smartWatch.Name);
            insertDeviceCommand.Parameters.AddWithValue("@IsOn", smartWatch.IsOn);
            
            insertDeviceResult = insertDeviceCommand.ExecuteNonQuery();
            if (insertDeviceResult == -1)
                throw new ApplicationException("Insert device failed.");
            
            SqlCommand insertWatchCommand = new SqlCommand(insertWatchQuery, connection);
            insertWatchCommand.Parameters.AddWithValue("@Id", count + 1);
            insertWatchCommand.Parameters.AddWithValue("@BatteryCharge", smartWatch.BatteryCharge);
            insertWatchCommand.Parameters.AddWithValue("@Device_id", smartWatch.Id);
            
            insertWatchResult = insertWatchCommand.ExecuteNonQuery();
            if (insertWatchResult == -1)
                throw new ApplicationException("Insert watch failed.");
        }
    }

    private void AddPersonalComputer(JsonNode? json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        PersonalComputer? personalComputer;
        try
        {
            personalComputer = JsonSerializer.Deserialize<PersonalComputer>(json, options);
        }
        catch
        {
            throw new ArgumentException("JSON deserialization failed. Seek help.");
        }
        
        if (personalComputer == null)
            throw new ArgumentException("JSON deserialization failed. Seek help.");
        
        // edgecases
        if (personalComputer.IsOn && personalComputer.OperatingSystem.IsNullOrEmpty())
            throw new ArgumentException("PC cannot be turned on without operating system.");
        
        // adding id and inserting into the db
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            var countPcQuery = "SELECT COUNT(*) FROM PersonalComputer";
            var count = -1;
            
            SqlCommand countCommand = new SqlCommand(countPcQuery, connection);
            connection.Open();
            SqlDataReader reader = countCommand.ExecuteReader();
            try
            {
                if (reader.Read())
                {
                    count = reader.GetInt32(0);
                }
            }
            finally
            {
                reader.Close();
            }
            personalComputer.Id = $"P-{count + 1}";

            var insertDeviceResult = -1;
            var insertComputerResult = -1;

            var insertDeviceQuery = $"INSERT INTO Device VALUES (@Id, @Name, @IsOn)";
            var insertComputerQuery = $"INSERT INTO PersonalComputer VALUES (@Id, @OperatingSystem, @Device_id)";
            SqlCommand insertDeviceCommand = new SqlCommand(insertDeviceQuery, connection);
            insertDeviceCommand.Parameters.AddWithValue("@Id", personalComputer.Id);
            insertDeviceCommand.Parameters.AddWithValue("@Name", personalComputer.Name);
            insertDeviceCommand.Parameters.AddWithValue("@IsOn", personalComputer.IsOn);
            
            insertDeviceResult = insertDeviceCommand.ExecuteNonQuery();
            if (insertDeviceResult == -1)
                throw new ApplicationException("Insert device failed.");
            
            SqlCommand insertComputerCommand = new SqlCommand(insertComputerQuery, connection);
            insertComputerCommand.Parameters.AddWithValue("@Id", count + 1);
            insertComputerCommand.Parameters.AddWithValue("@OperatingSystem", personalComputer.OperatingSystem);
            insertComputerCommand.Parameters.AddWithValue("@Device_id", personalComputer.Id);
            
            insertComputerResult = insertComputerCommand.ExecuteNonQuery();
            if (insertComputerResult == -1)
                throw new ApplicationException("Insert computer failed.");
        }
    }
    
    private void AddEmbeddedDevice(JsonNode? json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        EmbeddedDevice? embeddedDevice;
        try
        {
            embeddedDevice = JsonSerializer.Deserialize<EmbeddedDevice>(json, options);
        }
        catch
        {
            throw new ArgumentException("JSON deserialization failed. Seek help.");
        }
        
        if (embeddedDevice == null)
            throw new ArgumentException("JSON deserialization failed. Seek help.");
        
        // edge cases
        if (!Regex.IsMatch(embeddedDevice.IpAddress,
                @"^((25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\.){3}(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)$"))
        {
            throw new ArgumentException("IP address is not a valid IP address.");
        }

        if (!embeddedDevice.IsOn && embeddedDevice.IsConnected)
        {
            throw new ArgumentException("Device cannot be connected if it is turned off.");
        }

        if (embeddedDevice.IsOn && !embeddedDevice.NetworkName.Contains("MD Ltd."))
        {
            throw new ArgumentException("The network name should contain \"MD Ltd.\" for the device to be able to be connected.");
        }
        
        // adding id and inserting into the db
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            var countEdQuery = "SELECT COUNT(*) FROM EmbeddedDevice";
            var count = -1;
            
            SqlCommand countCommand = new SqlCommand(countEdQuery, connection);
            connection.Open();
            SqlDataReader reader = countCommand.ExecuteReader();
            try
            {
                if (reader.Read())
                {
                    count = reader.GetInt32(0);
                }
            }
            finally
            {
                reader.Close();
            }
            embeddedDevice.Id = $"ED-{count + 1}";

            var insertDeviceResult = -1;
            var insertEmbeddedResult = -1;

            var insertDeviceQuery = $"INSERT INTO Device VALUES (@Id, @Name, @IsOn)";
            var insertEmbeddedQuery = $"INSERT INTO EmbeddedDevice VALUES (@Id, @IpAddress, @NetworkName, @IsConnected, @Device_id)";
            SqlCommand insertDeviceCommand = new SqlCommand(insertDeviceQuery, connection);
            insertDeviceCommand.Parameters.AddWithValue("@Id", embeddedDevice.Id);
            insertDeviceCommand.Parameters.AddWithValue("@Name", embeddedDevice.Name);
            insertDeviceCommand.Parameters.AddWithValue("@IsOn", embeddedDevice.IsOn);
            
            insertDeviceResult = insertDeviceCommand.ExecuteNonQuery();
            if (insertDeviceResult == -1)
                throw new ApplicationException("Insert device failed.");
            
            SqlCommand insertEmbeddedCommand = new SqlCommand(insertEmbeddedQuery, connection);
            insertEmbeddedCommand.Parameters.AddWithValue("@Id", count + 1);
            insertEmbeddedCommand.Parameters.AddWithValue("@IpAddress", embeddedDevice.IpAddress);
            insertEmbeddedCommand.Parameters.AddWithValue("@NetworkName", embeddedDevice.NetworkName);
            insertEmbeddedCommand.Parameters.AddWithValue("@IsCOnnected", embeddedDevice.IsConnected);
            insertEmbeddedCommand.Parameters.AddWithValue("@Device_id", embeddedDevice.Id);
            
            insertEmbeddedResult = insertEmbeddedCommand.ExecuteNonQuery();
            if (insertEmbeddedResult == -1)
                throw new ApplicationException("Insert watch failed.");
        }
    }

    public bool UpdateSmartWatch(SmartWatch smartWatch)
    {
        throw new NotImplementedException();
    }

    public bool UpdatePersonalComputer(PersonalComputer personalComputer)
    {
        throw new NotImplementedException();
    }

    public bool UpdateEmbeddedDevice(EmbeddedDevice embeddedDevice)
    {
        throw new NotImplementedException();
    }

    public bool DeleteDevice(string id)
    {
        throw new NotImplementedException();
    }
}