using Devices.devices;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManagerWithRest.Controllers;

[ApiController]
[Route("[controller]")]
public class DeviceController : ControllerBase
{
    private static readonly List<Device> _devices = new();

    [HttpGet]
    [Route("/devices")]
    public IResult GetAllDevices()
    {
        var summaries = _devices.Select(d => new { d._id, d._name });
        return Results.Ok(summaries);
    }

    [HttpGet]
    [Route("/devices/{id}")]
    public IResult GetDeviceById(string id)
    {
        var device = _devices.FirstOrDefault(d => d._id == id);
        return device == null ? Results.NotFound() : Results.Json(device);
    }

    [HttpPost]
    [Route("/devices")]
    public IResult AddDevice([FromBody] Device device)
    {
        // check already taken id
        if (_devices.Any(d => d._id == device._id))
        {
            return Results.BadRequest("Device with this id already exists");
        }
        
        _devices.Add(device);
        return Results.Ok();
    }

    [HttpPut]
    [Route("/devices/{id}")]
    public IResult UpdateDevice(string id, [FromBody] Device updatedDevice)
    {
        // check indexes
        if (id != updatedDevice._id)
        {
            return Results.BadRequest("Id's in passed device and in the URL must match");
        }
        
        var index = _devices.FindIndex(d => d._id == id);
        if (index == -1)
        {
            return Results.NotFound("Device with this id doesn't exist");
        }

        _devices[index] = updatedDevice;

        return Results.Ok(updatedDevice);
    }

    [HttpDelete]
    [Route("/devices/{id}")]
    public IResult DeleteDevice(string id)
    {
        var device = _devices.FirstOrDefault(d => d._id == id);
        if (device != null) _devices.Remove(device);
        else
        {
            return Results.NotFound("Device with this id doesn't exist");
        }
        
        return Results.Ok(device);
    }
}