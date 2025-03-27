using DeviceManager.devices;

namespace DeviceManager.deviceManager;

public interface IDeviceManager
{
    Device? GetDeviceById(string id);

    void ShowAllDevices();

    void AddDevice(Device device);

    void EditDevice(string id, Device updatedDevice);

    void RemoveDevice(string id);

    void SaveToFile();
}