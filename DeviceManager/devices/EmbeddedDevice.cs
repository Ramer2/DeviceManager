using System;
using System.Text.RegularExpressions;
using ArgumentException = DeviceManager.exceptions.ArgumentException;

namespace DeviceManager.devices;

public class EmbeddedDevice : Device
{
    public string IpAddress { get; private set; }
    private string _networkName { get; set; }

    public EmbeddedDevice(int id, string name, bool isOn, string ip, string network) : base(id, name, isOn)
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
            throw new Exception("ConnectionException: Network not allowed.");
        
        Console.WriteLine($"{_name} connected successfully.");
    }

    public override void TurnOn()
    {
        Connect();
        base.TurnOn();
    }
}