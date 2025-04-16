using Devices.exceptions;

namespace Devices.devices;

/// <summary>
/// Represents a smartwatch device with battery management.
/// </summary>
public class SmartWatch : Device, IPowerNotifier
{
    
    /// <summary>
    /// Battery charge percentage.
    /// </summary>
    public int _batteryCharge
    {
        get => _batteryCharge;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(
                    "Can't set the battery charge to a negative value or to a value greater than 100");

            _batteryCharge = value;

            if (_batteryCharge < 20) NotifyLowPower();
        }
    }
    
    /// <summary>
    /// Initializes a new instance of the SmartWatch class.
    /// </summary>
    /// <param name="_id">The unique device ID.</param>
    /// <param name="_name">The name of the device.</param>
    /// <param name="_isOn">Indicates whether the device is initially turned on.</param>
    /// <param name="_batteryCharge">The initial battery charge level.</param>
    public SmartWatch(string _id, string _name, bool _isOn, int _batteryCharge) : base(_id, _name, _isOn)
    {
        this._batteryCharge = _batteryCharge;
    }
    
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
