﻿using Devices.devices;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManagerWithRest.Controllers;

[ApiController]
[Route("[controller]")]
public class DeviceController : ControllerBase
{
    private static readonly List<Device> Devices = [];

    [HttpGet]
    [Route("/devices")]
    public IResult GetAllDevices()
    {
        var summaries = Devices.Select(d => new { d.Id, d.Name });
        return Results.Ok(summaries);
    }

    [HttpGet]
    [Route("/devices/{id}")]
    public IResult GetDeviceById(string id)
    {
        var device = Devices.FirstOrDefault(d => d.Id == id);
        return device == null ? Results.NotFound() : Results.Json(device);
    }

    [HttpPost]
    [Route("/devices/smartWatches")]
    public IResult AddsSmartWatch([FromBody] SmartWatch smartWatch)
    {
        // check already taken id
        if (Devices.Any(d => d.Id == smartWatch.Id))
        {
            return Results.BadRequest("Device with this id already exists");
        }
        
        Devices.Add(smartWatch);
        return Results.Ok();
    }

    [HttpPost]
    [Route("/devices/personalComputers")]
    public IResult AddsPersonalComputer([FromBody] PersonalComputer personalComputer)
    {
        // check already taken id
        if (Devices.Any(d => d.Id == personalComputer.Id))
        {
            return Results.BadRequest("Device with this id already exists");
        }
        
        Devices.Add(personalComputer);
        return Results.Ok();
    }

    [HttpPost]
    [Route("/devices/embeddedDevices")]
    public IResult AddsEmbeddedDevice([FromBody] EmbeddedDevice embeddedDevice)
    {
        // check already taken id
        if (Devices.Any(d => d.Id == embeddedDevice.Id))
        {
            return Results.BadRequest("Device with this id already exists");
        }
        
        Devices.Add(embeddedDevice);
        return Results.Ok();
    }

    [HttpPut]
    [Route("/devices/{id}")]
    public IResult UpdateDevice(string id, [FromBody] Device updatedDevice)
    {
        // check indexes
        if (id != updatedDevice.Id)
        {
            return Results.BadRequest("Id's in passed device and in the URL must match");
        }
        
        var index = Devices.FindIndex(d => d.Id == id);
        if (index == -1)
        {
            return Results.NotFound("Device with this id doesn't exist");
        }

        Devices[index] = updatedDevice;

        return Results.Ok(updatedDevice);
    }

    [HttpDelete]
    [Route("/devices/{id}")]
    public IResult DeleteDevice(string id)
    {
        var device = Devices.FirstOrDefault(d => d.Id == id);
        if (device != null) Devices.Remove(device);
        else
        {
            return Results.NotFound("Device with this id doesn't exist");
        }
        
        return Results.Ok(device);
    }
}