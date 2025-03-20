using DeviceManager.devices;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeviceManager.Test;

[TestClass]
[TestSubject(typeof(DeviceManager))]
public class DeviceManagerTest
{

    [TestMethod]
    public void TestNewDeviceAdded()
    {
        var dm = new DeviceManager("input.txt");
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
        var dm = new DeviceManager("input.txt");
        dm.EditDevice("SW-1", new SmartWatch("SW-1", "SW", false, 1));
        var device = (SmartWatch)dm.GetDeviceById("SW-1");
        Assert.AreEqual(device._name, "SW");
    }
}