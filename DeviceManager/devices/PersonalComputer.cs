using System;

namespace DeviceManager.devices;

public class PersonalComputer : Device
{
    private string _operatingSystem { get; set; }

    public PersonalComputer(int id, string name, bool isOn, string os) : base(id, name, isOn)
    {
        _operatingSystem = os;
    }

    public override void TurnOn()
    {
        if (string.IsNullOrEmpty(_operatingSystem))
            throw new Exception("EmptySystemException: No OS installed.");
        
        base.TurnOn();
    }
}