using System.Text.Json.Nodes;
using DeviceManager.Application.dtos;
using Devices.devices;

namespace DeviceManager.Application;

public interface IDeviceService
{
    public IEnumerable<DeviceDTO> GetAllDevices();
    
    public Device? GetDeviceById(string id);

    public bool AddDeviceByJson(JsonNode? json);
    
    public bool AddDeviceByRawText(string text);
    
    public bool UpdateDevice(JsonNode? json);
    
    public bool DeleteDevice(string id);
}