using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                if (!int.TryParse(parts[1].Trim(), out var id))
                {
                    Console.WriteLine($"Skipping corrupted line (invalid ID): {line}");
                    continue;
                }

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
                            _devices.Add(new SmartWatch(id, name, isOn, battery));
                        }
                        else
                        {
                            var os = parts.Length > 4 ? parts[4].Trim() : "NoOS";
                            _devices.Add(new PersonalComputer(id, name, isOn, os));
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

                        _devices.Add(new EmbeddedDevice(id, name, false, ip, network));
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
    
    public void EditDevice(string deviceType, int id, Device updatedDevice)
    {
        var existingDevice = _devices.FirstOrDefault(d => d._id == id);
        if (existingDevice == null)
        {
            Console.WriteLine($"No device found with ID {id}.");
            return;
        }
        
        _devices.Remove(existingDevice);
        updatedDevice._id = id;
        switch (deviceType)
        {
            case "SW":
                if (updatedDevice is SmartWatch sw)
                {
                    if (sw.BatteryCharge < 0 || sw.BatteryCharge > 100)
                    {
                        Console.WriteLine("Invalid battery value. Must be between 0 and 100.");
                        return;
                    }
                    _devices.Add(sw);
                }
                else
                {
                    Console.WriteLine("Invalid device type. Expected a SmartWatch.");
                }
                break;

            case "P":
                if (updatedDevice is PersonalComputer pc)
                {
                    _devices.Add(pc);
                }
                break;

            case "ED":
                if (updatedDevice is EmbeddedDevice ed)
                {
                    if (!Regex.IsMatch(ed.IpAddress, @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$"))
                    {
                        Console.WriteLine("Invalid IP format.");
                        return;
                    }
                    _devices.Add(ed);
                }
                break;

            default:
                Console.WriteLine("Unknown device type.");
                return;
        }

        Console.WriteLine($"Device ID {id} successfully updated.");
    }


    public void AddDevice(Device device)
    {
        if (_devices.Count >= MaxDevices)
            throw new Exception("Device storage full.");
        _devices.Add(device);
    }

    public void RemoveDevice(int id)
    {
        _devices.RemoveAll(d => d._id == id);
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
            var writer = new StreamWriter(_filePath, true);
            foreach (var device in _devices)
            {
                writer.WriteLine(device.ToString());
            }
            Console.WriteLine("Devices successfully saved to file.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to file: {ex.Message}");
        }
    }

}