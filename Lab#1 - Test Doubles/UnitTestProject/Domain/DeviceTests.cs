using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartHomeExample;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
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
    }
}
