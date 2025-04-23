using Devices.exceptions;

namespace Devices.devices;

/// <summary>
/// Represents a smartwatch device with battery management.
/// </summary>
public class SmartWatch : Device, IPowerNotifier
{
    
    private int _batteryCharge;

    public int BatteryCharge
    {
        get => _batteryCharge;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(BatteryCharge), 
                    "Can't set the battery charge to a negative value or to a value greater than 100");

            _batteryCharge = value;

            if (_batteryCharge < 20) NotifyLowPower();
        }
    }
    
    /// <summary>
    /// Initializes a new instance of the SmartWatch class.
    /// </summary>
    /// <param name="id">The unique device ID.</param>
    /// <param name="name">The name of the device.</param>
    /// <param name="isOn">Indicates whether the device is initially turned on.</param>
    /// <param name="batteryCharge">The initial battery charge level.</param>
    public SmartWatch(string id, string name, bool isOn, int batteryCharge) : base(id, name, isOn)
    {
        _batteryCharge = batteryCharge;
    }
    
    public SmartWatch() : base("", "", false) { }
    
    /// <summary>
    /// Turns the device on.
    /// </summary>
    public override void TurnOn()
    {
        if (_batteryCharge < 11)
            throw new EmptyBatteryException();
        
        base.TurnOn();
        _batteryCharge -= 10;
    }

    /// <summary>
    /// Notifies the user of low battery power.
    /// </summary>
    public void NotifyLowPower()
    {
        Console.WriteLine("Low Power (less than 20)");
    }
    
    /// <summary>
    /// Returns a string representation of the device.
    /// </summary>
    /// <returns>A formatted string containing device details.</returns>
    public override string ToString()
    {
        return $"{base.ToString()} - {_batteryCharge}";
    }
}
