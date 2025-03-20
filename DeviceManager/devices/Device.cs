namespace DeviceManager.devices;

public class Device
{
    protected internal int _id {get; set;}
    protected string _name {get; set;}
    protected bool _isOn {get; set;}

    public Device(int id, string name, bool isOn)
    {
        _id = id;
        _name = name;
        _isOn = isOn;
    }
    
    public virtual void TurnOn() => _isOn = true;
    public virtual void TurnOff() => _isOn = false;

    public override string ToString()
    {
        return $"{_id} - {_name} - {(_isOn ? "On" : "Off")}";
    }
}