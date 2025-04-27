using Devices.exceptions;

namespace Devices.devices;

/// <summary>
/// Represents a personal computer device.
/// </summary>
public class PersonalComputer : Device
{
    public int Id { get; set; }
    public string OperatingSystem { get; set; }
    
    public string Device_Id { get; set; }

    /// <summary>
    /// Initializes a new instance of the PersonalComputer class.
    /// </summary>
    /// <param name="id">The unique personal computer ID.</param>
    /// <param name="name">The name of the device.</param>
    /// <param name="isOn">Indicates whether the device is initially turned on.</param>
    /// <param name="operatingSystem">The operating system of the computer.</param>
    /// <param name="deviceId">The unique device ID.</param>
    public PersonalComputer(int id, string name, bool isOn, string operatingSystem, string deviceId) : base(deviceId, name, isOn)
    {
        Id = id;
        OperatingSystem = operatingSystem;
        Device_Id = deviceId;
    }
    
    public PersonalComputer() : base("", "", false) { }

    /// <summary>
    /// Turns on the computer, ensuring the operating system is set.
    /// </summary>
    public override void TurnOn()
    {
        if (string.IsNullOrEmpty(OperatingSystem))
            throw new EmptySystemException();
        
        base.TurnOn();
    }
    
    /// <summary>
    /// Returns a string representation of the device.
    /// </summary>
    /// <returns>A formatted string containing device details.</returns>
    public override string ToString()
    {
        return $"{base.ToString()} - {OperatingSystem}";
    }
}