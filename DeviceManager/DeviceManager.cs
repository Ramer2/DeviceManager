using System.Text.RegularExpressions;
using DeviceManager.devices;

namespace DeviceManager;

public class DeviceManager
{
    private List<Device> _devices = new List<Device>();
    private const int MaxDevices = 15;
    private string _filePath;

    public DeviceManager(string path)
    {
        _filePath = path;
        LoadDevices();
    }

    private void LoadDevices()
    {
        if (!File.Exists(_filePath)) return;

        foreach (var line in File.ReadAllLines(_filePath))
        {
            try
            {
                var fixedLine = line.Replace("-", ",");
                var parts = fixedLine.Split(',');

                if (parts.Length < 3)
                {
                    Console.WriteLine($"Skipping corrupted line (too few fields): {line}");
                    continue;
                }

                var type = parts[0].Trim();
                if (!int.TryParse(parts[1].Trim(), out var idNumber))
                {
                    Console.WriteLine($"Skipping corrupted line (invalid ID): {line}");
                    continue;
                }

                string id = $"{type}-{idNumber}";

                var name = parts[2].Trim();
                switch (type)
                {
                    case "SW":
                    case "P":
                        if (parts.Length < 4)
                        {
                            Console.WriteLine($"Skipping corrupted line (missing IsOn value): {line}");
                            continue;
                        }

                        if (!bool.TryParse(parts[3].Trim(), out var isOn))
                        {
                            Console.WriteLine($"Skipping corrupted line (invalid IsOn value): {line}");
                            continue;
                        }

                        if (type == "SW")
                        {
                            if (parts.Length < 5)
                            {
                                Console.WriteLine($"Skipping corrupted line (missing battery data): {line}");
                                continue;
                            }

                            var batteryStr = parts[4].Replace("%", "").Trim();
                            if (!int.TryParse(batteryStr, out var battery))
                            {
                                Console.WriteLine($"Skipping corrupted line (invalid battery percentage): {line}");
                                continue;
                            }

                            AddDevice(new SmartWatch(id, name, isOn, battery));
                        }
                        else
                        {
                            var os = parts.Length > 4 ? parts[4].Trim() : "NoOS";
                            AddDevice(new PersonalComputer(id, name, isOn, os));
                        }

                        break;
                    case "ED":
                        if (parts.Length < 5)
                        {
                            Console.WriteLine($"Skipping corrupted line (missing network data): {line}");
                            continue;
                        }

                        var ip = parts[3].Trim();
                        var network = parts[4].Trim();

                        if (!Regex.IsMatch(ip, @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$"))
                        {
                            Console.WriteLine($"Skipping corrupted line (invalid IP format): {line}");
                            continue;
                        }

                        AddDevice(new EmbeddedDevice(id, name, false, ip, network));
                        break;

                    default:
                        Console.WriteLine($"Skipping corrupted line (unknown device type): {line}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Skipping corrupted line: {line}. Error: {ex.Message}");
            }
        }
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

    public Device? GetDeviceById(string id)
    {
        return _devices.FirstOrDefault(d => d._id == id);
    }
}
