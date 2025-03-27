using DeviceManager.devices;

namespace DeviceManager.deviceManager;

public class DeviceManager : IDeviceManager
{
    private readonly List<Device> _devices = [];
    private const int MaxDevices = 15;
    private readonly string _filePath;

    private DeviceManager(string path)
    {
        _filePath = path;
        LoadDevices();
    }

    public static DeviceManager Create(string path)
    {
        return new DeviceManager(path);
    }

    private void LoadDevices()
    {
        if (!File.Exists(_filePath)) return;

        foreach (var line in File.ReadAllLines(_filePath))
        {
            var parts = line.Split(',');

            var device = DeviceFactory.CreateDevice(parts);
            
            if (device != null) AddDevice(device);
        }
    }
    
    public Device? GetDeviceById(string id)
    {
        return _devices.FirstOrDefault(d => d._id == id);
    }

    public void AddDevice(Device device)
    {
        if (_devices.Count >= MaxDevices)
        {
            Console.WriteLine("Device storage full.");
            return;
        }
    
        if (_devices.Any(d => d._id == device._id))
        {
            Console.WriteLine($"Device with ID {device._id} already exists. Cannot add duplicate.");
            return;
        }
    
        _devices.Add(device);
        Console.WriteLine($"Device {device._id} successfully added.");
    }

    public void EditDevice(string id, Device updatedDevice)
    {
        var existingDevice = _devices.FirstOrDefault(d => d._id == id);
        if (existingDevice == null)
        {
            Console.WriteLine($"No device found with ID {id}.");
            return;
        }
        
        _devices.Remove(existingDevice);
        updatedDevice._id = id;
        
        AddDevice(updatedDevice);
        Console.WriteLine($"Device {id} successfully updated.");
    }

    public void RemoveDevice(string id)
    {
        _devices.RemoveAll(d => d._id == id);
        Console.WriteLine($"Device {id} removed.");
    }

    public void ShowAllDevices()
    {
        foreach (var device in _devices)
            Console.WriteLine(device);
    }

    public void SaveToFile()
    {
        try
        {
            using (var writer = new StreamWriter(_filePath, false))
            {
                foreach (var device in _devices)
                {
                    writer.WriteLine(device.ToString());
                }
            }
            Console.WriteLine("Devices successfully saved to file.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to file: {ex.Message}");
        }
    }
}

public static class DeviceManagerFactory
{
    public static IDeviceManager CreateDeviceManager(string filePath)
    {
        return DeviceManager.Create(filePath);
    }
}