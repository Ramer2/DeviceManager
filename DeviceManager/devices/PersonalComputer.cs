using DeviceManager.exceptions;

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
            throw new EmptySystemException();
        
        base.TurnOn();
    }
    
    public override string ToString()
    {
        return $"{base.ToString()} - {_operatingSystem}";
    }
}