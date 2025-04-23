using DeviceManager.Application.dtos;
using Devices.devices;
using Microsoft.Data.SqlClient;

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
        string query = "SELECT * FROM Device";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            if (id.Contains("SW"))
            {
                query += " JOIN SmartWatch ON Device.Id = SmartWatch.Device_id WHERE SmartWatch.Id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    if (reader.HasRows)
                    {
                        return new SmartWatch
                        {
                            Id = reader.GetString(0),
                            Name = reader.GetString(1),
                            IsOn = reader.GetBoolean(2),
                            BatteryCharge = reader.GetByte(3)
                        };
                    }
                }
                finally
                {
                    reader.Close();
                }
            } else if (id.Contains("P"))
            {
                query += " JOIN PersonalComputer ON Device.Id = PersonalComputer.Device_id WHERE PersonalComputer.Id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    if (reader.HasRows)
                    {
                        return new PersonalComputer
                        {
                            Id = reader.GetString(0),
                            Name = reader.GetString(1),
                            IsOn = reader.GetBoolean(2),
                            OperatingSystem = reader.GetString(3)
                        };
                    }
                }
                finally
                {
                    reader.Close();
                }
            } else if (id.Contains("ED"))
            {
                query += " JOIN dbo.EmbeddedDevice ED on Device.Id = ED.Device_id WHERE Device.Id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    if (reader.HasRows)
                    {
                        return new EmbeddedDevice
                        {
                            Id = reader.GetString(0),
                            Name = reader.GetString(1),
                            IsOn = reader.GetBoolean(2),
                            IpAddress = reader.GetString(3),
                            NetworkName = reader.GetString(4)
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

    public bool AddSmartWatch(SmartWatch smartWatch)
    {
        // const string query = "INSERT INTO SmartWatch VALUES (@id, @name, @isOn);";
        throw new NotImplementedException();
    }

    public bool AddPersonalComputer(PersonalComputer personalComputer)
    {
        throw new NotImplementedException();
    }

    public bool AddEmbeddedDevice(EmbeddedDevice embeddedDevice)
    {
        throw new NotImplementedException();
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