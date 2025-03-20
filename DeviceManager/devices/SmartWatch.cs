using DeviceManager.exceptions;

namespace DeviceManager.devices;

public class SmartWatch : Device, IPowerNotifier
{
    private int _batteryCharge;
    
    public int BatteryCharge
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
    
    public SmartWatch(string id, string name, bool isOn, int batteryCharge) : base(id, name, isOn)
    {
        if (batteryCharge < 0 || batteryCharge > 100)
            throw new ArgumentOutOfRangeException(
                "Can't set the battery charge to a negative value or to a value greater than 100");
        
        _batteryCharge = batteryCharge;
    }

    public override void TurnOn()
    {
        if (_batteryCharge < 11)
            throw new EmptyBatteryException();
        
        base.TurnOn();
        _batteryCharge -= 10;
    }

    public void NotifyLowPower()
    {
        Console.WriteLine("Low Power (less than 20)");
    }
    
    public override string ToString()
    {
        return $"{base.ToString()} - {_batteryCharge}";
    }
}