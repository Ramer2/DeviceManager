using DeviceManager.devices;
using DeviceManager.exceptions;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeviceManager.Test.devices;

[TestClass]
[TestSubject(typeof(EmbeddedDevice))]
public class EmbeddedDeviceTest
{
    [TestMethod]
    public void TestTurnOnException()
    {
        Assert.Throws<ConnectionException>(() =>
        {
            var ed = new EmbeddedDevice("ED-5", "ED", false, "127.0.0.1", "localhost");
            ed.TurnOn();
        });
    }
}