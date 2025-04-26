using System.Text.Json.Nodes;
using DeviceManager.Application.dtos;
using Devices.devices;
using Microsoft.AspNetCore.Http;

namespace DeviceManager.Application;

public interface IDeviceService
{
    public IEnumerable<DeviceDTO> GetAllDevices();
    
    public Device GetDeviceById(string id);

    public bool AddDeviceByJson(JsonNode? json);
    
    public bool UpdateSmartWatch(SmartWatch smartWatch);
    
    public bool UpdatePersonalComputer(PersonalComputer personalComputer);
    
    public bool UpdateEmbeddedDevice(EmbeddedDevice embeddedDevice);

    public bool DeleteDevice(string id);
}