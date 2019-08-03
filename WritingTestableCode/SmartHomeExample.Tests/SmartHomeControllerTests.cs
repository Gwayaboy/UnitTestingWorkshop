using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SmartHomeExample.Tests
{
    [TestClass]
    public class SmartHomeControllerTests
    { 
        /**
         * GetTimeOfDay's non-deterministic behavior makes it impossible to test the 
         * internal logic of the GetTimeOfDay()
         * method without actually changing the system date and time.
         * 
         * This is no longer a unit test as it requires specific/environement configuration setup.
         */

         [TestMethod]
        public void GetTimeOfDay_At1PM_ReturnsMorning()
        {
            try
            {
                // Arrange
                // Has to change system time to 1PM
                var sut = new SmartHomeController();

                // Act
                var timeOfDay = sut.GetTimeOfDay();

                // Assert
                Assert.AreEqual(TimeOfDay.Afternoon, timeOfDay);
            }
            finally
            {
                // Teardown: roll system time back
            }
        }


        [TestMethod]
        public void GetTimeOfDay_For6AM_ReturnsMorning()
        {
            // Arrange phase is empty: testing static method, nothing to initialize
            var sut = new SmartHomeController();

            // Act
            var timeOfDay = sut.GetTimeOfDay(new DateTime(2019, 08, 06, 06, 00, 00));

            // Assert
            Assert.AreEqual(TimeOfDay.Morning, timeOfDay);
        }


        [TestMethod]
        public void ActuateLights_MotionDetectedAtNight_TurnsOnTheLight()
        {
            // Arrange: create a pair of actions that change boolean variable instead of really turning the light on or off.
            bool turnedOn = false;
            Action turnOn = () => turnedOn = true;
            Action turnOff = () => turnedOn = false;
            SmartHomeController sut = null;
            //sut = new SmartHomeController(new FakeDateTimeProvider(23, 59, 59));

            // Act
            sut.ActuateLights(true, turnOn, turnOff);

            // Assert
            Assert.IsTrue(turnedOn);
        }

    }

}
