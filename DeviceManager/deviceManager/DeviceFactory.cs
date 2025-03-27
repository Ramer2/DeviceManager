using System.Text.RegularExpressions;
using DeviceManager.devices;

namespace DeviceManager.deviceManager;

public static class DeviceFactory
{
    public static Device? CreateDevice(string[] parts)
    {
        if (parts.Length < 3)
        {
            Console.WriteLine($"Skipping corrupted line (too few fields): {string.Join(",", parts)}");
            return null;
        }
        
        var id = parts[0].Trim();
        var name = parts[1].Trim();
        
        try
        {
            switch (id.Split('-')[0])
            {
                case "SW":
                    if (parts.Length < 4) return null;
                    if (!bool.TryParse(parts[2].Trim(), out var isOn)) return null;
                    if (!int.TryParse(parts[3].Replace("%", "").Trim(), out var battery)) return null;
                    return new SmartWatch(id, name, isOn, battery);

                case "P":
                    if (!bool.TryParse(parts[2].Trim(), out isOn)) return null;
                    var os = parts.Length > 3 ? parts[3].Trim() : "NoOS";
                    return new PersonalComputer(id, name, isOn, os);

                case "ED":
                    if (parts.Length < 4) return null;
                    var ip = parts[2].Trim();
                    var network = parts[3].Trim();
                    if (!Regex.IsMatch(ip, @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$")) return null;
                    return new EmbeddedDevice(id, name, false, ip, network);

                default:
                    Console.WriteLine($"Skipping unknown device type: {id}");
                    return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating device: {ex.Message}");
            return null;
        }
    }
}