namespace Devices;

/// <summary>
/// Interface describing a method for notifying the user
/// about device being low power.
/// </summary>
public interface IPowerNotifier
{
    /// <summary>
    /// Method notifying the user.
    /// </summary>
    void NotifyLowPower();
}