using System.Text.RegularExpressions;
using Devices.exceptions;
using ArgumentException = Devices.exceptions.ArgumentException;

namespace Devices.devices;

/// <summary>
/// Represents an embedded device with network connectivity.
/// </summary>
public class EmbeddedDevice : Device
{
    /// <summary>
    /// IP address of the embedded device.
    /// </summary>
    public string IpAddress { get; set; }
    
    public string NetworkName { get; }

    /// <summary>
    /// Indicator for whether the device is connected to a network.
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// Initializes a new instance of the EmbeddedDevice class.
    /// </summary>
    /// <param name="id">The unique device ID.</param>
    /// <param name="name">The name of the device.</param>
    /// <param name="isOn">Indicates whether the device is initially turned on.</param>
    /// <param name="ipAddress">The IP address of the device.</param>
    /// <param name="networkName">The network name.</param>
    public EmbeddedDevice(string id, string name, bool isOn, string ipAddress, string networkName) : base(id, name, isOn)
    {
        SetIpAddress(ipAddress);
        NetworkName = networkName;
        IpAddress = ipAddress;
        IsConnected = false;
    }

    /// <summary>
    /// Sets the IP address of the device.
    /// </summary>
    /// <param name="ip">The new IP address.</param>
    public void SetIpAddress(string ip)
    {
        if (!Regex.IsMatch(ip, @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$"))
            throw new ArgumentException();
        
        IpAddress = ip;
    }

    /// <summary>
    /// Connects the device to the network.
    /// </summary>
    public void Connect()
    {
        if (!NetworkName.Contains("MD Ltd."))
            throw new ConnectionException();
        
        IsConnected = true;
        Console.WriteLine($"{Name} connected successfully.");
    }

    /// <summary>
    /// Disconnects the device from the network.
    /// </summary>
    public void Disconnect()
    {
        if (!IsConnected)
        {
            Console.WriteLine($"{Name} is already disconnected.");
            return;
        } 
        IsConnected = false;
        Console.WriteLine($"{Name} was disconnected.");
    }
    
    /// <summary>
    /// Turns the device on.
    /// </summary>
    public override void TurnOn()
    {
        Connect();
        base.TurnOn();
    }

    /// <summary>
    /// Turns the device off.
    /// </summary>
    public override void TurnOff()
    {
        Disconnect();
        base.TurnOff();
    }

    /// <summary>
    /// Returns a string representation of the device.
    /// </summary>
    /// <returns>A formatted string containing device details.</returns>
    public override string ToString()
    {
        return $"{base.ToString()} - {IpAddress} - {NetworkName}";
    }
}