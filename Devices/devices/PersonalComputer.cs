using Devices.exceptions;

namespace Devices.devices;

/// <summary>
/// Represents a personal computer device.
/// </summary>
public class PersonalComputer : Device
{
    private string _operatingSystem { get; set; }

    /// <summary>
    /// Initializes a new instance of the PersonalComputer class.
    /// </summary>
    /// <param name="_id">The unique device ID.</param>
    /// <param name="_name">The name of the device.</param>
    /// <param name="_isOn">Indicates whether the device is initially turned on.</param>
    /// <param name="_operatingSystem">The operating system of the computer.</param>
    public PersonalComputer(string _id, string _name, bool _isOn, string _operatingSystem) : base(_id, _name, _isOn)
    {
        this._operatingSystem = _operatingSystem;
    }

    /// <summary>
    /// Turns on the computer, ensuring the operating system is set.
    /// </summary>
    public override void TurnOn()
    {
        if (string.IsNullOrEmpty(_operatingSystem))
            throw new EmptySystemException();
        
        base.TurnOn();
    }
    
    /// <summary>
    /// Returns a string representation of the device.
    /// </summary>
    /// <returns>A formatted string containing device details.</returns>
    public override string ToString()
    {
        return $"{base.ToString()} - {_operatingSystem}";
    }
}