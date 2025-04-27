using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using DeviceManager.Application.dtos;
using Devices.devices;
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
    
    public bool AddDeviceByJson(JsonNode? json)
    {
        var deviceType = json["deviceType"]?.ToString();
        if (string.IsNullOrEmpty(deviceType))
        {
            throw new ArgumentException("Invalid JSON format. deviceType is not specified.");
        }
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        switch (deviceType.ToLower())
        {
            case "smartwatch":
            {
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
                
                AddSmartWatch(smartWatch);
                break;
            }
            
            case "personalcomputer":
            {
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
                
                AddPersonalComputer(personalComputer);
                break;
            }
            
            case "embeddeddevice":
            {
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
                
                AddEmbeddedDevice(embeddedDevice);
                break;
            }

            default:
                throw new ArgumentException("Uknown device type.");
        }
        
        return false;
    }
    
    private void AddSmartWatch(SmartWatch smartWatch)
    {
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
            
            // set the device id only if it was not set
            if (smartWatch.Id.IsNullOrEmpty())
            {
                smartWatch.Id = $"SW-{count + 1}";
            }

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

    private void AddPersonalComputer(PersonalComputer personalComputer)
    {
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
            
            // set the device id only if it was not set
            if (personalComputer.Id.IsNullOrEmpty())
            {
                personalComputer.Id = $"P-{count + 1}";
            }

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
            insertComputerCommand.Parameters.AddWithValue("@OperatingSystem", personalComputer.OperatingSystem ?? "");
            insertComputerCommand.Parameters.AddWithValue("@Device_id", personalComputer.Id);
            
            insertComputerResult = insertComputerCommand.ExecuteNonQuery();
            if (insertComputerResult == -1)
                throw new ApplicationException("Insert computer failed.");
        }
    }
    
    private void AddEmbeddedDevice(EmbeddedDevice embeddedDevice)
    {
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
            
            // set the device id only if it was not set
            if (embeddedDevice.Id.IsNullOrEmpty())
            {
                embeddedDevice.Id = $"ED-{count + 1}";
            }

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

    public bool AddDeviceByRawText(string text)
    {
        var parts = text.Split(',');
        switch (parts[0].Split('-')[0])
        {
            case "SW":
            {
                SmartWatch smartWatch = new SmartWatch();
                smartWatch.Id = parts[0];
                smartWatch.Name = parts[1];
                try
                {
                    smartWatch.IsOn = bool.Parse(parts[2]);
                }
                catch
                {
                    throw new ArgumentException("Invalid boolean value for IsOn parameter.");
                }
                try
                {
                    smartWatch.BatteryCharge = int.Parse(parts[3]);
                }
                catch
                {
                    throw new ArgumentException("Invalid int value for BatteryCharge parameter.");
                }
                AddSmartWatch(smartWatch);
                break;
            }
            case "P":
            {
                PersonalComputer personalComputer = new PersonalComputer();
                personalComputer.Id = parts[0];
                personalComputer.Name = parts[1];
                try
                {
                    personalComputer.IsOn = bool.Parse(parts[2]);
                }
                catch
                {
                    throw new ArgumentException("Invalid boolean value for IsOn parameter.");
                }

                if (parts.Length > 3)
                {
                    personalComputer.OperatingSystem = parts[3];
                }
                AddPersonalComputer(personalComputer);
                break;
            }
            case "ED":
            {
                EmbeddedDevice embeddedDevice = new EmbeddedDevice();
                embeddedDevice.Id = parts[0];
                embeddedDevice.Name = parts[1];
                embeddedDevice.IsOn = false;
                embeddedDevice.IpAddress = parts[2];
                embeddedDevice.NetworkName = parts[3];
                embeddedDevice.IsConnected = false;
                AddEmbeddedDevice(embeddedDevice);
                break;
            }
            default: throw new ArgumentException("Unknown device.");
        }

        return true;
    }

    public bool UpdateDevice(JsonNode? json)
    {
        var id = json["id"]?.ToString();
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        if (id.Contains("SW"))
        {
            SmartWatch? smartWatch;
            try
            {
                smartWatch = JsonSerializer.Deserialize<SmartWatch>(json, options);
            }
            catch
            {
                throw new ArgumentException("JSON serialization failed. Seek help.");
            }
            
            if (smartWatch == null)
                throw new ArgumentException("JSON serialization failed. Seek help.");
            
            UpdateSmartWatch(smartWatch);
        } else if (id.Contains("P"))
        {
            PersonalComputer? personalComputer;
            try
            {
                personalComputer = JsonSerializer.Deserialize<PersonalComputer>(json, options);
            }
            catch
            {
                throw new ArgumentException("JSON serialization failed. Seek help.");
            }
            
            if (personalComputer == null)
                throw new ArgumentException("JSON serialization failed. Seek help.");
            
            UpdatePersonalComputer(personalComputer);
        }
        else if (id.Contains("ED"))
        {
            EmbeddedDevice? embeddedDevice;
            try
            {
                embeddedDevice = JsonSerializer.Deserialize<EmbeddedDevice>(json, options);
            }
            catch
            {
                throw new ArgumentException("JSON serialization failed. Seek help.");
            }
            
            if (embeddedDevice == null)
                throw new ArgumentException("JSON serialization failed. Seek help.");
            
            UpdateEmbeddedDevice(embeddedDevice);
        }
        else
        {
            throw new ArgumentException("Uknown device type.");
        }

        return true;
    }

    private void UpdateSmartWatch(SmartWatch smartWatch)
    {
        // edgecases
        if (smartWatch.BatteryCharge is < 0 or > 100)
            throw new ArgumentException("JSON deserialization failed. Battery charge is out of range [0 - 100].");
        
        // updating the whole object
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            var updateDeviceResult = -1;
            var updateWatchResult = -1;

            var updateDeviceQuery = "UPDATE Device SET IsOn = @IsOn, Name = @Name WHERE Id = @Id";
            var updateWatchQuery = "UPDATE SmartWatch SET BatteryCharge = @BatteryCharge WHERE Device_id = @Id";

            connection.Open();
            
            SqlCommand updateDeviceCommand = new SqlCommand(updateDeviceQuery, connection);
            updateDeviceCommand.Parameters.AddWithValue("@Id", smartWatch.Id);
            updateDeviceCommand.Parameters.AddWithValue("@IsOn", smartWatch.IsOn);
            updateDeviceCommand.Parameters.AddWithValue("@Name", smartWatch.Name);
            
            updateDeviceResult = updateDeviceCommand.ExecuteNonQuery();
            if (updateDeviceResult == -1)
                throw new ApplicationException("Updating device failed.");

            SqlCommand updateWatchCommand = new SqlCommand(updateWatchQuery, connection);
            updateWatchCommand.Parameters.AddWithValue("@Id", smartWatch.Id);
            updateWatchCommand.Parameters.AddWithValue("@BatteryCharge", smartWatch.BatteryCharge);

            updateWatchResult = updateWatchCommand.ExecuteNonQuery();
            if (updateWatchResult == -1)
                throw new ApplicationException("Updating device failed.");
        }
    }
    
    private void UpdatePersonalComputer(PersonalComputer personalComputer)
    {
        // edgecases
        if (personalComputer.IsOn && personalComputer.OperatingSystem.IsNullOrEmpty())
            throw new ArgumentException("PC cannot be turned on without operating system.");
        
        // updating the whole object
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            var updateDeviceResult = -1;
            var updateWatchResult = -1;

            var updateDeviceQuery = "UPDATE Device SET IsOn = @IsOn, Name = @Name WHERE Id = @Id";
            var updateWatchQuery = "UPDATE PersonalComputer SET OperatingSystem = @OperatingSystem WHERE Device_id = @Id";

            connection.Open();
            
            SqlCommand updateDeviceCommand = new SqlCommand(updateDeviceQuery, connection);
            updateDeviceCommand.Parameters.AddWithValue("@Id", personalComputer.Id);
            updateDeviceCommand.Parameters.AddWithValue("@IsOn", personalComputer.IsOn);
            updateDeviceCommand.Parameters.AddWithValue("@Name", personalComputer.Name);
            
            updateDeviceResult = updateDeviceCommand.ExecuteNonQuery();
            if (updateDeviceResult == -1)
                throw new ApplicationException("Updating device failed.");

            SqlCommand updateWatchCommand = new SqlCommand(updateWatchQuery, connection);
            updateWatchCommand.Parameters.AddWithValue("@Id", personalComputer.Id);
            updateWatchCommand.Parameters.AddWithValue("@OperatingSystem", personalComputer.OperatingSystem);

            updateWatchResult = updateWatchCommand.ExecuteNonQuery();
            if (updateWatchResult == -1)
                throw new ApplicationException("Updating device failed.");
        }
    }
    
    private void UpdateEmbeddedDevice(EmbeddedDevice embeddedDevice)
    {
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
        
        throw new NotImplementedException();
    }

    public bool DeleteDevice(string id)
    {
        throw new NotImplementedException();
    }
}