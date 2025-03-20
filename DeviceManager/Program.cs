using DeviceManager.devices;

var filePath = "input.txt";
var dm = new DeviceManager.DeviceManager(filePath);

dm.ShowAllDevices();
Console.WriteLine();

// add a new device
var newWatch = new SmartWatch("SW-5", "Fitbit Versa", false, 75);
dm.AddDevice(newWatch);
Console.WriteLine();

// edit device
var updatedPC = new PersonalComputer("P-2", "Updated ThinkPad", false, "Windows 11");
dm.EditDevice("P-2", updatedPC);
Console.WriteLine();

dm.ShowAllDevices();
Console.WriteLine();

// Save changes to file
dm.SaveToFile();