namespace Devices.devices;

/// <summary>
/// Represents a generic device with basic properties and functionality.
/// </summary>
public abstract class Device
{
    /// <summary>
    /// Unique identifier of the device.
    /// </summary>
    public string Id {get; set;}
    
    /// <summary>
    /// Name of the device.
    /// </summary>
    public string Name {get; set;}
    
    /// <summary>
    /// Indicator for whether the device is turned on.
    /// </summary>
    public bool IsOn {get; set;}
    
    /// <summary>
    /// Row version timestamp.
    /// </summary>
    public byte[] DeviceRowVersion { get; set; }

    /// <summary>
    /// Initializes a new instance of the Device class.
    /// </summary>
    /// <param name="id">The unique device ID.</param>
    /// <param name="name">The name of the device.</param>
    /// <param name="isOn">Indicator for whether the device is initially turned on.</param>
    protected Device(string id, string name, bool isOn)
    {
        Id = id;
        Name = name;
        IsOn = isOn;
    }
    
    /// <summary>
    /// Turns the device on.
    /// </summary>
    public virtual void TurnOn() => IsOn = true;
    
    /// <summary>
    /// Turns the device off.
    /// </summary>
    public virtual void TurnOff() => IsOn = false;

    /// <summary>
    /// Returns a string representation of the device.
    /// </summary>
    /// <returns>A formatted string containing device details.</returns>
    public override string ToString()
    {
        return $"{Id} - {Name} - {(IsOn ? "On" : "Off")}";
    }
}