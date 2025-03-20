using DeviceManager.devices;
using DeviceManager.exceptions;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeviceManager.Test.devices;

[TestClass]
[TestSubject(typeof(PersonalComputer))]
public class PersonalComputerTest
{

    [TestMethod]
    public void TestEmptyOS()
    {
        Assert.Throws<EmptySystemException>(() =>
        {
            var pc = new PersonalComputer("P-5", "PC", false, null);
            pc.TurnOn();
        });
    }
}