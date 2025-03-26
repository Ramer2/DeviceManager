using DeviceManager.devices;

// loading devices
var filePath = "input.txt";
var dm = new DeviceManager.DeviceManager(filePath);

// displaying all devices
try
{
    dm.ShowAllDevices();
    Console.WriteLine();
}
catch (Exception e)
{
    Console.WriteLine("Exception when displaying all devices");
}

try
{
    // add a new device
    var newWatch = new SmartWatch("SW-5", "Fitbit Versa", false, 75);
    dm.AddDevice(newWatch);
    Console.WriteLine();
}
catch (Exception e)
{
    Console.WriteLine("Exception when adding a new device");
}

try
{
    // edit device
    var updatedPC = new PersonalComputer("P-2", "Updated ThinkPad", false, "Windows 11");
    dm.EditDevice("P-2", updatedPC);
    Console.WriteLine();

    dm.ShowAllDevices();
    Console.WriteLine();
}
catch (Exception e)
{
    Console.WriteLine("Exception when editing a device");
}

try
{
    // Save changes to file
    dm.SaveToFile();
}
catch (Exception e)
{
    Console.WriteLine("Exception when saving devices");
}