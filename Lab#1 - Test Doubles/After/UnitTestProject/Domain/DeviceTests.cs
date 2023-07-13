using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SmartHomeExample;
using System;
using System.Data;
using TestDoubles.Domain;

namespace UnitTestProject.Domain
{
    [TestClass]
    public class DeviceTests
    {

        [TestMethod]
        public void GetTimeOfDay_For6AM_ReturnsMorning()
        {
            // Arrange phase is empty: testing static method, nothing to initialize
            var sut = new Device(Guid.NewGuid(),"MyTV",null);

            // Act
            var timeOfDay = sut.GetTimeOfDay(new DateTime(2019, 08, 06, 06, 00, 00));

            // Assert
            Assert.AreEqual(TimeOfDay.Morning, timeOfDay);
        }

        [TestMethod]
        [DataRow(2, TimeOfDay.Night)]
        [DataRow(6,TimeOfDay.Morning)]
        [DataRow(13,TimeOfDay.Afternoon)]
        [DataRow(19, TimeOfDay.Evening)]

        public void GetTimeOfDay_ForGivenDateTime_ReturnsCorrespondingSectionOfDay(int hour, TimeOfDay expected)
        {
            //Arrange
            var sut = new Device(Guid.NewGuid(), "SomeDevice", null);

            //Act
            var result = sut.GetTimeOfDay(new DateTime(2019, 08, 06, hour, 0, 0));

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void UpdateFirmware_FailingToSetDeviceOnline_DoesNotUpdateFirmWare()
        {
            //Arrange
            var factoryFirware = new Firmware();
            var sut = Substitute.ForPartsOf<Device>(Guid.NewGuid(), "SomeDevice", factoryFirware, true);
            var newFirmware = new Firmware();

            sut
              .When(x => x.SetOnLine(true))
              .Do(x => throw new Exception("Could not turn device on"));

            //Act
            sut.UpdateFirmware(newFirmware, DateTime.Now);


            //Assert
            Assert.AreEqual(sut.CurrentFirmware, factoryFirware);
        }
    }
}
