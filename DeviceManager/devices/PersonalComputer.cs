using System;

namespace DeviceManager.devices;

public class PersonalComputer : Device
{
    private string _operatingSystem { get; set; }

    public PersonalComputer(string id, string name, bool isOn, string os) : base(id, name, isOn)
    {
        _operatingSystem = os;
    }

    public override void TurnOn()
    {
        if (string.IsNullOrEmpty(_operatingSystem))
            throw new Exception("EmptySystemException: No OS installed.");
        
        base.TurnOn();
    }
    
    public override string ToString()
    {
        return $"{base.ToString()} - {_operatingSystem}";
    }
}