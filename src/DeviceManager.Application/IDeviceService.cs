using DeviceManager.Application.dtos;
using Devices.devices;

namespace DeviceManager.Application;

public interface IDeviceService
{
    public IEnumerable<DeviceDTO> GetAllDevices();
    
    public Device GetDeviceById(string id);

    public bool AddDevice(Device device);
    
    public bool UpdateSmartWatch(SmartWatch smartWatch);
    
    public bool UpdatePersonalComputer(PersonalComputer personalComputer);
    
    public bool UpdateEmbeddedDevice(EmbeddedDevice embeddedDevice);

    public bool DeleteDevice(string id);
}