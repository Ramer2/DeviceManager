﻿using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using DeviceManager.Application.dtos;
using DeviceManager.Repository;
using Devices.devices;
using Microsoft.IdentityModel.Tokens;

namespace DeviceManager.Application;

public class DeviceService : IDeviceService
{
    private IDatabaseRepository _databaseRepository;

    public DeviceService(IDatabaseRepository databaseRepository)
    {
        _databaseRepository = databaseRepository;
    }
    
    public IEnumerable<DeviceDTO> GetAllDevices()
    {
        return _databaseRepository.GetAllDevices();
    }

    public Device? GetDeviceById(string id)
    {
        return _databaseRepository.GetDeviceById(id);
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
                
                // edgecases
                if (smartWatch.BatteryCharge is < 0 or > 100)
                    throw new ArgumentException("JSON deserialization failed. Battery charge is out of range [0 - 100].");
                
                _databaseRepository.AddSmartWatch(smartWatch);
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
                
                // edgecases
                if (personalComputer.IsOn && personalComputer.OperatingSystem.IsNullOrEmpty())
                    throw new ArgumentException("PC cannot be turned on without operating system.");
                
                _databaseRepository.AddPersonalComputer(personalComputer);
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
                
                _databaseRepository.AddEmbeddedDevice(embeddedDevice);
                break;
            }

            default:
                throw new ApplicationException("Uknown device type.");
        }
        
        return false;
    }

    public bool AddDeviceByRawText(string text)
    {
        var parts = text.Split(',');
        switch (parts[0].Split('-')[0])
        {
            case "SW":
            {
                SmartWatch smartWatch = new SmartWatch();
                smartWatch.Device_Id = parts[0];
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
                    smartWatch.BatteryCharge = int.Parse(parts[3].Replace("%", ""));
                }
                catch
                {
                    throw new ArgumentException("Invalid int value for BatteryCharge parameter.");
                }
                
                // edgecases
                if (smartWatch.BatteryCharge is < 0 or > 100)
                    throw new ArgumentException("JSON deserialization failed. Battery charge is out of range [0 - 100].");
                
                _databaseRepository.AddSmartWatch(smartWatch);
                break;
            }
            case "P":
            {
                PersonalComputer personalComputer = new PersonalComputer();
                personalComputer.Device_Id = parts[0];
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
                else
                {
                    personalComputer.OperatingSystem = "";
                }
                
                // edgecases
                if (personalComputer.IsOn && personalComputer.OperatingSystem.IsNullOrEmpty())
                    throw new ArgumentException("PC cannot be turned on without operating system.");
                
                _databaseRepository.AddPersonalComputer(personalComputer);
                break;
            }
            case "ED":
            {
                EmbeddedDevice embeddedDevice = new EmbeddedDevice();
                embeddedDevice.Device_Id = parts[0];
                embeddedDevice.Name = parts[1];
                embeddedDevice.IsOn = false;
                embeddedDevice.IpAddress = parts[2];
                embeddedDevice.NetworkName = parts[3];
                embeddedDevice.IsConnected = false;
                
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
                
                _databaseRepository.AddEmbeddedDevice(embeddedDevice);
                break;
            }
            default: throw new ArgumentException("Unknown device.");
        }

        return true;
    }

    public bool UpdateDevice(JsonNode? json)
    {
        var id = json["device_id"]?.ToString();
        if (id.IsNullOrEmpty())
            throw new ArgumentException("Invalid or not specified id.");

        if (GetDeviceById(id) == null)
        {
            throw new FileNotFoundException("Device not found.");
        }

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
            
            // edgecases
            if (smartWatch.BatteryCharge is < 0 or > 100)
                throw new ArgumentException("JSON deserialization failed. Battery charge is out of range [0 - 100].");
            
            _databaseRepository.UpdateSmartWatch(smartWatch);
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
            
            // edgecases
            if (personalComputer.IsOn && personalComputer.OperatingSystem.IsNullOrEmpty())
                throw new ArgumentException("PC cannot be turned on without operating system.");
            
            _databaseRepository.UpdatePersonalComputer(personalComputer);
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

            if (embeddedDevice.IsConnected && !embeddedDevice.NetworkName.Contains("MD Ltd."))
            {
                throw new ArgumentException("The network name should contain \"MD Ltd.\" for the device to be able to be connected.");
            }
            
            _databaseRepository.UpdateEmbeddedDevice(embeddedDevice);
        }
        else
        {
            throw new ApplicationException("Uknown device type.");
        }

        return true;
    }

    public bool DeleteDevice(string id)
    {
        if (GetDeviceById(id) == null)
        {
            throw new FileNotFoundException("Device not found.");
        }
        
        if (id.Contains("SW")) _databaseRepository.DeleteWatch(id);
        else if (id.Contains("P")) _databaseRepository.DeleteComputer(id);
        else if (id.Contains("ED")) _databaseRepository.DeleteEmbeddedDevice(id);
        else throw new ApplicationException("Unknown device type.");

        return true;
    }
}