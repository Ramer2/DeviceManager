using System;
using System.Text.RegularExpressions;
using DeviceManager.exceptions;
using ArgumentException = DeviceManager.exceptions.ArgumentException;

namespace DeviceManager.devices;

public class EmbeddedDevice : Device
{
    public string IpAddress { get; private set; }
    private string _networkName { get; set; }

    public EmbeddedDevice(string id, string name, bool isOn, string ip, string network) : base(id, name, isOn)
    {
        SetIPAddress(ip);
        _networkName = network;
    }

    public void SetIPAddress(string ip)
    {
        if (!Regex.IsMatch(ip, @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$"))
            throw new ArgumentException();
        
        IpAddress = ip;
    }

    public void Connect()
    {
        if (!_networkName.Contains("MD Ltd."))
            throw new ConnectionException();
        
        Console.WriteLine($"{_name} connected successfully.");
    }

    public override void TurnOn()
    {
        Connect();
        base.TurnOn();
    }

    public override string ToString()
    {
        return $"{base.ToString()} - {IpAddress} - {_networkName}";
    }
}