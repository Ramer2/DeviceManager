using DeviceManager.Application.dtos;
using Devices.devices;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DeviceManager.Repository;

public class DatabaseRepository : IDatabaseRepository
{
    private string _connectionString;

    public DatabaseRepository(string connectionString)
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
                            Device_Id = reader.GetString(0),
                            Name = reader.GetString(1),
                            IsOn = reader.GetBoolean(2),
                            Id = reader.GetInt32(3),
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
                            Device_Id = reader.GetString(0),
                            Name = reader.GetString(1),
                            IsOn = reader.GetBoolean(2),
                            Id = reader.GetInt32(3),
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
                            Device_Id = reader.GetString(0),
                            Name = reader.GetString(1),
                            IsOn = reader.GetBoolean(2),
                            Id = reader.GetInt32(3),
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
    
    public void AddSmartWatch(SmartWatch smartWatch)
    {
        // adding id and inserting into the db
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                var countSwQuery = "SELECT MAX(id) FROM SmartWatch";
                var count = -1;
                SqlCommand countCommand = new SqlCommand(countSwQuery, connection, transaction);
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
                if (smartWatch.Device_Id.IsNullOrEmpty())
                {
                    smartWatch.Device_Id = $"SW-{count + 1}";
                }

                var insertDeviceResult = -1;
                var insertWatchResult = -1;

                var insertDeviceQuery = $"INSERT INTO Device VALUES (@Id, @Name, @IsOn)";
                var insertWatchQuery = $"INSERT INTO SmartWatch VALUES (@Id, @BatteryCharge, @Device_id)";
                SqlCommand insertDeviceCommand = new SqlCommand(insertDeviceQuery, connection, transaction);
                insertDeviceCommand.Parameters.AddWithValue("@Id", smartWatch.Device_Id);
                insertDeviceCommand.Parameters.AddWithValue("@Name", smartWatch.Name);
                insertDeviceCommand.Parameters.AddWithValue("@IsOn", smartWatch.IsOn);

                insertDeviceResult = insertDeviceCommand.ExecuteNonQuery();
                if (insertDeviceResult == -1)
                    throw new ApplicationException("Insert device failed.");

                SqlCommand insertWatchCommand = new SqlCommand(insertWatchQuery, connection, transaction);
                insertWatchCommand.Parameters.AddWithValue("@Id", count + 1);
                insertWatchCommand.Parameters.AddWithValue("@BatteryCharge", smartWatch.BatteryCharge);
                insertWatchCommand.Parameters.AddWithValue("@Device_id", smartWatch.Device_Id);

                insertWatchResult = insertWatchCommand.ExecuteNonQuery();
                if (insertWatchResult == -1)
                    throw new ApplicationException("Insert watch failed.");
                
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    public void AddPersonalComputer(PersonalComputer personalComputer)
    {
        // adding id and inserting into the db
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                var countPcQuery = "SELECT MAX(id) FROM PersonalComputer";
                var count = -1;
                SqlCommand countCommand = new SqlCommand(countPcQuery, connection, transaction);
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
                if (personalComputer.Device_Id.IsNullOrEmpty())
                {
                    personalComputer.Device_Id = $"P-{count + 1}";
                }

                var insertDeviceResult = -1;
                var insertComputerResult = -1;

                var insertDeviceQuery = $"INSERT INTO Device VALUES (@Id, @Name, @IsOn)";
                var insertComputerQuery = $"INSERT INTO PersonalComputer VALUES (@Id, @OperatingSystem, @Device_id)";
                SqlCommand insertDeviceCommand = new SqlCommand(insertDeviceQuery, connection, transaction);
                insertDeviceCommand.Parameters.AddWithValue("@Id", personalComputer.Device_Id);
                insertDeviceCommand.Parameters.AddWithValue("@Name", personalComputer.Name);
                insertDeviceCommand.Parameters.AddWithValue("@IsOn", personalComputer.IsOn);
                
                insertDeviceResult = insertDeviceCommand.ExecuteNonQuery();
                if (insertDeviceResult == -1)
                    throw new ApplicationException("Insert device failed.");
                
                SqlCommand insertComputerCommand = new SqlCommand(insertComputerQuery, connection, transaction);
                insertComputerCommand.Parameters.AddWithValue("@Id", count + 1);
                insertComputerCommand.Parameters.AddWithValue("@OperatingSystem", personalComputer.OperatingSystem);
                insertComputerCommand.Parameters.AddWithValue("@Device_id", personalComputer.Device_Id);
                
                insertComputerResult = insertComputerCommand.ExecuteNonQuery();
                if (insertComputerResult == -1)
                    throw new ApplicationException("Insert computer failed.");
                
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
    
    public void AddEmbeddedDevice(EmbeddedDevice embeddedDevice)
    {
        // adding id and inserting into the db
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                var countEdQuery = "SELECT MAX(id) FROM EmbeddedDevice";
                var count = -1;
                SqlCommand countCommand = new SqlCommand(countEdQuery, connection, transaction);
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
                if (embeddedDevice.Device_Id.IsNullOrEmpty())
                {
                    embeddedDevice.Device_Id = $"ED-{count + 1}";
                }

                var insertDeviceResult = -1;
                var insertEmbeddedResult = -1;

                var insertDeviceQuery = $"INSERT INTO Device VALUES (@Id, @Name, @IsOn)";
                var insertEmbeddedQuery = $"INSERT INTO EmbeddedDevice VALUES (@Id, @IpAddress, @NetworkName, @IsConnected, @Device_id)";
                SqlCommand insertDeviceCommand = new SqlCommand(insertDeviceQuery, connection, transaction);
                insertDeviceCommand.Parameters.AddWithValue("@Id", embeddedDevice.Device_Id);
                insertDeviceCommand.Parameters.AddWithValue("@Name", embeddedDevice.Name);
                insertDeviceCommand.Parameters.AddWithValue("@IsOn", embeddedDevice.IsOn);
                
                insertDeviceResult = insertDeviceCommand.ExecuteNonQuery();
                if (insertDeviceResult == -1)
                    throw new ApplicationException("Insert device failed.");
                
                SqlCommand insertEmbeddedCommand = new SqlCommand(insertEmbeddedQuery, connection, transaction);
                insertEmbeddedCommand.Parameters.AddWithValue("@Id", count + 1);
                insertEmbeddedCommand.Parameters.AddWithValue("@IpAddress", embeddedDevice.IpAddress);
                insertEmbeddedCommand.Parameters.AddWithValue("@NetworkName", embeddedDevice.NetworkName);
                insertEmbeddedCommand.Parameters.AddWithValue("@IsCOnnected", embeddedDevice.IsConnected);
                insertEmbeddedCommand.Parameters.AddWithValue("@Device_id", embeddedDevice.Device_Id);
                
                insertEmbeddedResult = insertEmbeddedCommand.ExecuteNonQuery();
                if (insertEmbeddedResult == -1)
                    throw new ApplicationException("Insert watch failed.");
                
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
    
    public void UpdateSmartWatch(SmartWatch smartWatch)
    {
        // updating the whole object
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                var updateDeviceResult = -1;
                var updateWatchResult = -1;

                var updateDeviceQuery = "UPDATE Device SET IsOn = @IsOn, Name = @Name WHERE Id = @Id";
                var updateWatchQuery = "UPDATE SmartWatch SET BatteryCharge = @BatteryCharge WHERE Device_id = @Id";
            
                SqlCommand updateDeviceCommand = new SqlCommand(updateDeviceQuery, connection, transaction);
                updateDeviceCommand.Parameters.AddWithValue("@Id", smartWatch.Device_Id);
                updateDeviceCommand.Parameters.AddWithValue("@IsOn", smartWatch.IsOn);
                updateDeviceCommand.Parameters.AddWithValue("@Name", smartWatch.Name);
            
                updateDeviceResult = updateDeviceCommand.ExecuteNonQuery();
                if (updateDeviceResult == -1)
                    throw new ApplicationException("Updating device failed.");

                SqlCommand updateWatchCommand = new SqlCommand(updateWatchQuery, connection, transaction);
                updateWatchCommand.Parameters.AddWithValue("@Id", smartWatch.Device_Id);
                updateWatchCommand.Parameters.AddWithValue("@BatteryCharge", smartWatch.BatteryCharge);

                updateWatchResult = updateWatchCommand.ExecuteNonQuery();
                if (updateWatchResult == -1)
                    throw new ApplicationException("Updating device failed.");
                
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
    
    public void UpdatePersonalComputer(PersonalComputer personalComputer)
    {
        // updating the whole object
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                var updateDeviceResult = -1;
                var updateWatchResult = -1;

                var updateDeviceQuery = "UPDATE Device SET IsOn = @IsOn, Name = @Name WHERE Id = @Id";
                var updateWatchQuery = "UPDATE PersonalComputer SET OperatingSystem = @OperatingSystem WHERE Device_id = @Id";

                SqlCommand updateDeviceCommand = new SqlCommand(updateDeviceQuery, connection, transaction);
                updateDeviceCommand.Parameters.AddWithValue("@Id", personalComputer.Device_Id);
                updateDeviceCommand.Parameters.AddWithValue("@IsOn", personalComputer.IsOn);
                updateDeviceCommand.Parameters.AddWithValue("@Name", personalComputer.Name);
            
                updateDeviceResult = updateDeviceCommand.ExecuteNonQuery();
                if (updateDeviceResult == -1)
                    throw new ApplicationException("Updating device failed.");

                SqlCommand updateWatchCommand = new SqlCommand(updateWatchQuery, connection, transaction);
                updateWatchCommand.Parameters.AddWithValue("@Id", personalComputer.Device_Id);
                updateWatchCommand.Parameters.AddWithValue("@OperatingSystem", personalComputer.OperatingSystem);

                updateWatchResult = updateWatchCommand.ExecuteNonQuery();
                if (updateWatchResult == -1)
                    throw new ApplicationException("Updating device failed.");
                
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
    
    public void UpdateEmbeddedDevice(EmbeddedDevice embeddedDevice)
    {
// updating the whole object
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                var updateDeviceResult = -1;
                var updateWatchResult = -1;

                var updateDeviceQuery = "UPDATE Device SET IsOn = @IsOn, Name = @Name WHERE Id = @Id";
                var updateWatchQuery = "UPDATE EmbeddedDevice SET IpAddress = @IpAddress, NetworkName = @NetworkName, IsConnected = @IsConnected WHERE Device_id = @Id";

                SqlCommand updateDeviceCommand = new SqlCommand(updateDeviceQuery, connection, transaction);
                updateDeviceCommand.Parameters.AddWithValue("@Id", embeddedDevice.Device_Id);
                updateDeviceCommand.Parameters.AddWithValue("@IsOn", embeddedDevice.IsOn);
                updateDeviceCommand.Parameters.AddWithValue("@Name", embeddedDevice.Name);
            
                updateDeviceResult = updateDeviceCommand.ExecuteNonQuery();
                if (updateDeviceResult == -1)
                    throw new ApplicationException("Updating device failed.");

                SqlCommand updateWatchCommand = new SqlCommand(updateWatchQuery, connection, transaction);
                updateWatchCommand.Parameters.AddWithValue("@Id", embeddedDevice.Device_Id);
                updateWatchCommand.Parameters.AddWithValue("@IpAddress", embeddedDevice.IpAddress);
                updateWatchCommand.Parameters.AddWithValue("@NetworkName", embeddedDevice.NetworkName);
                updateWatchCommand.Parameters.AddWithValue("@IsConnected", embeddedDevice.IsConnected);

                updateWatchResult = updateWatchCommand.ExecuteNonQuery();
                if (updateWatchResult == -1)
                    throw new ApplicationException("Updating device failed.");
                
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
    
    public void DeleteWatch(string id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                var deleteWatchResult = -1;
                var deleteDeviceResult = -1;

                var deleteWatchQuery = "DELETE FROM SmartWatch WHERE Device_Id = @Device_Id";
                var deleteDeviceQuery = "DELETE FROM Device WHERE Id = @Id";

                SqlCommand deleteWatchCommand = new SqlCommand(deleteWatchQuery, connection, transaction);
                deleteWatchCommand.Parameters.AddWithValue("@Device_Id", id);
                deleteWatchResult = deleteWatchCommand.ExecuteNonQuery();

                if (deleteWatchResult == -1)
                    throw new ApplicationException("Deleting the device failed.");
                
                SqlCommand deleteDeviceCommand = new SqlCommand(deleteDeviceQuery, connection, transaction);
                deleteDeviceCommand.Parameters.AddWithValue("@Id", id);
                deleteDeviceResult = deleteDeviceCommand.ExecuteNonQuery();
                
                if (deleteDeviceResult == -1)
                    throw new ApplicationException("Deleting the device failed.");
                
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
    
    public void DeleteComputer(string id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                var deleteComputerResult = -1;
                var deleteDeviceResult = -1;

                var deleteComputerQuery = "DELETE FROM PersonalComputer WHERE Device_Id = @Device_Id";
                var deleteDeviceQuery = "DELETE FROM Device WHERE Id = @Id";

                SqlCommand deleteComputerCommand = new SqlCommand(deleteComputerQuery, connection, transaction);
                deleteComputerCommand.Parameters.AddWithValue("@Device_Id", id);
                deleteComputerResult = deleteComputerCommand.ExecuteNonQuery();

                if (deleteComputerResult == -1)
                    throw new ApplicationException("Deleting the device failed.");
                
                SqlCommand deleteDeviceCommand = new SqlCommand(deleteDeviceQuery, connection, transaction);
                deleteDeviceCommand.Parameters.AddWithValue("@Id", id);
                deleteDeviceResult = deleteDeviceCommand.ExecuteNonQuery();
                
                if (deleteDeviceResult == -1)
                    throw new ApplicationException("Deleting the device failed.");
                
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
    
    public void DeleteEmbeddedDevice(string id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                var deleteEmbeddedResult = -1;
                var deleteDeviceResult = -1;

                var deleteEmbeddedQuery = "DELETE FROM EmbeddedDevice WHERE Device_Id = @Device_Id";
                var deleteDeviceQuery = "DELETE FROM Device WHERE Id = @Id";

                connection.Open();
                SqlCommand deleteEmbeddedCommand = new SqlCommand(deleteEmbeddedQuery, connection);
                deleteEmbeddedCommand.Parameters.AddWithValue("@Device_Id", id);
                deleteEmbeddedResult = deleteEmbeddedCommand.ExecuteNonQuery();

                if (deleteEmbeddedResult == -1)
                    throw new ApplicationException("Deleting the device failed.");
                
                SqlCommand deleteDeviceCommand = new SqlCommand(deleteDeviceQuery, connection);
                deleteDeviceCommand.Parameters.AddWithValue("@Id", id);
                deleteDeviceResult = deleteDeviceCommand.ExecuteNonQuery();
                
                if (deleteDeviceResult == -1)
                    throw new ApplicationException("Deleting the device failed.");
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}