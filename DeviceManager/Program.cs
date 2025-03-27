using DeviceManager;
using DeviceManager.deviceManager;
using DeviceManager.devices;

// loading devices
const string filePath = "input.txt";
var dm = DeviceManagerFactory.CreateDeviceManager(filePath);

// displaying all devices
try
{
    dm.ShowAllDevices();
    Console.WriteLine();
}
catch (Exception e)
{
    Console.WriteLine("Exception when displaying all devices");
    Console.WriteLine(e.Message);
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
    Console.WriteLine(e.Message);
}

try
{
    // edit device
    var updatedPc = new PersonalComputer("P-2", "Updated ThinkPad", false, "Windows 11");
    dm.EditDevice("P-2", updatedPc);
    Console.WriteLine();

    dm.ShowAllDevices();
    Console.WriteLine();
}
catch (Exception e)
{
    Console.WriteLine("Exception when editing a device");
    Console.WriteLine(e.Message);
}

try
{
    // Save changes to file
    dm.SaveToFile();
}
catch (Exception e)
{
    Console.WriteLine("Exception when saving devices");
    Console.WriteLine(e.Message);
}