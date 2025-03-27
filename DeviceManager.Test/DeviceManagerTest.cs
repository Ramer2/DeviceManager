using DeviceManager.deviceManager;
using DeviceManager.devices;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeviceManager.Test.devices;

[TestClass]
[TestSubject(typeof(deviceManager.DeviceManager))]
public class DeviceManagerTest
{

    [TestMethod]
    public void TestNewDeviceAdded()
    {
        var dm = DeviceManagerFactory.CreateDeviceManager("input.txt");
        var newDevice = new SmartWatch("SW-5", "SW", false, 13);
        dm.AddDevice(newDevice);

        Assert.IsNotNull(() =>
        {
            dm.GetDeviceById("SW-5");
        });
    }

    [TestMethod]
    public void TestDeviceEdited()
    {
        var dm = DeviceManagerFactory.CreateDeviceManager("input.txt");
        dm.EditDevice("SW-1", new SmartWatch("SW-1", "SW", false, 1));
        var device = (SmartWatch)dm.GetDeviceById("SW-1");
        Assert.AreEqual("SW-1", device?._id);
    }
}