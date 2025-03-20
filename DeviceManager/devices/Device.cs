namespace DeviceManager.devices;

public class Device
{
    public string _id {get; set;}
    public string _name {get; set;}
    public bool _isOn {get; set;}

    public Device(string id, string name, bool isOn)
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