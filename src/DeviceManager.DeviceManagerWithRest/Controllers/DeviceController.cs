﻿using System.Text.Json.Nodes;
using DeviceManager.Application;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManagerWithRest.Controllers;

[ApiController]
[Route("/api/devices/[controller]")]
public class DeviceController : ControllerBase
{
    private IDeviceService _deviceService;

    public DeviceController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet]
    [Route("/api/devices")]
    public IResult GetAllDevices()
    {
        var devices = _deviceService.GetAllDevices();
        var deviceDtos = devices.ToList();
        if (!deviceDtos.Any()) return Results.NotFound();
        return Results.Ok(deviceDtos);
    }
    
    [HttpGet]
    [Route("/api/devices/{id}")]
    public IResult GetDeviceById(string id)
    {
        var device = _deviceService.GetDeviceById(id);
        if (device == null) return Results.NotFound();
        return Results.Json(device);
    }
    
    [HttpPost]
    [Route("/api/devices")]
    [Consumes("application/json", "text/plain")]
    public async Task<IResult> AddDevice()
    {
        var contentType = Request.ContentType?.ToLower();

        switch (contentType)
        {
            case "application/json":
            {
                using var reader = new StreamReader(Request.Body);
                string rawJson = await reader.ReadToEndAsync();

                var json = JsonNode.Parse(rawJson);
                if (json == null)
                    return Results.BadRequest("Invalid JSON format.");

                try
                {
                    _deviceService.AddDeviceByJson(json);
                }
                catch (Exception e)
                {
                    return Results.BadRequest(e.Message);
                }
                return Results.Created();
            }

            case "text/plain":
            {
                using var reader = new StreamReader(Request.Body);
                var rawText = await reader.ReadToEndAsync();

                try
                {
                    _deviceService.AddDeviceByRawText(rawText);
                }
                catch (Exception e)
                {
                    return Results.BadRequest(e.Message);
                }
                return Results.Created();
            }
            default:
                return Results.Conflict("Unsupported Content-Type.");
        }
    }
    
    [HttpPut]
    [Route("/api/devices")]
    public async Task<IResult> UpdateDevice()
    {
        var contentType = Request.ContentType?.ToLower();

        switch (contentType)
        {
            case "application/json":
            {
                using var reader = new StreamReader(Request.Body);
                string rawJson = await reader.ReadToEndAsync();

                var json = JsonNode.Parse(rawJson);
                if (json == null)
                    return Results.BadRequest("Invalid JSON format.");

                try
                {
                    _deviceService.UpdateDevice(json);
                }
                catch (FileNotFoundException e)
                {
                    return Results.NotFound(e.Message);
                }
                catch (Exception e)
                {
                    return Results.BadRequest(e.Message);
                }
                return Results.Ok();
            }
            default:
                return Results.Conflict("Unsupported Content-Type.");
        }
    }
    
    [HttpDelete]
    [Route("/api/devices/{id}")]
    public IResult DeleteDeviceById(string id)
    {
        try
        {
            _deviceService.DeleteDevice(id);
        }
        catch (FileNotFoundException e)
        {
            return Results.NotFound(e.Message);
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
        return Results.Ok();
    }
}