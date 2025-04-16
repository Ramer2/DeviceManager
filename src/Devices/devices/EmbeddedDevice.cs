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
    public string IpAddress { get; private set; }
    
    private string _networkName { get; set; }

    /// <summary>
    /// Indicator for whether the device is connected to a network.
    /// </summary>
    private bool _isConnected { get; set; }

    /// <summary>
    /// Initializes a new instance of the EmbeddedDevice class.
    /// </summary>
    /// <param name="_id">The unique device ID.</param>
    /// <param name="_name">The name of the device.</param>
    /// <param name="_isOn">Indicates whether the device is initially turned on.</param>
    /// <param name="_ip">The IP address of the device.</param>
    /// <param name="_network">The network name.</param>
    public EmbeddedDevice(string _id, string _name, bool _isOn, string _ip, string _networkName) : base(_id, _name, _isOn)
    {
        SetIpAddress(_ip);
        this._networkName = _networkName;
        _isConnected = false;
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
        if (!_networkName.Contains("MD Ltd."))
            throw new ConnectionException();
        
        _isConnected = true;
        Console.WriteLine($"{_name} connected successfully.");
    }

    /// <summary>
    /// Disconnects the device from the network.
    /// </summary>
    public void Disconnect()
    {
        if (!_isConnected)
        {
            Console.WriteLine($"{_name} is already disconnected.");
            return;
        } 
        _isConnected = false;
        Console.WriteLine($"{_name} was disconnected.");
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
        return $"{base.ToString()} - {IpAddress} - {_networkName}";
    }
}