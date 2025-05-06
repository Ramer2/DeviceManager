using DeviceManager.Application.dtos;
using Devices.devices;

namespace DeviceManager.Repository;

public interface IDatabaseRepository
{
    public IEnumerable<DeviceDTO> GetAllDevices();
    
    public Device GetDeviceById(string id);

    public void AddSmartWatch(SmartWatch smartWatch);

    public void AddPersonalComputer(PersonalComputer personalComputer);
    
    public void AddEmbeddedDevice(EmbeddedDevice embeddedDevice);

    public void UpdateSmartWatch(SmartWatch smartWatch);

    public void UpdatePersonalComputer(PersonalComputer personalComputer);
    
    public void UpdateEmbeddedDevice(EmbeddedDevice embeddedDevice);

    public void DeleteWatch(string id);

    public void DeleteComputer(string id);
    
    public void DeleteEmbeddedDevice(string id);

}