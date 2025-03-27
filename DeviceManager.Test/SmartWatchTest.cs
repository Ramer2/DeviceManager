using System;
using DeviceManager.devices;
using DeviceManager.exceptions;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeviceManager.Test.devices;

[TestClass]
[TestSubject(typeof(SmartWatch))]
public class SmartWatchTest
{
    [TestMethod]
    public void TestIllegalArgumentForWatch()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var smartWatch = new SmartWatch("SW-4", "SW", false, -7);
            Console.WriteLine(smartWatch.ToString());
        });
    }
    
    
    [TestMethod]
    public void TestBatteryTooLow()
    {
        Assert.Throws<EmptyBatteryException>(() =>
        {
            var watch = new SmartWatch("SW-5", "SW", false, 10);
            watch.TurnOn();
        });
    }
}