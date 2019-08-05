using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using TestDoubles;
using TestDoubles.Domain;
using TestDoubles.Interfaces;

namespace UnitTestProject
{
    //When introducing Mocking framework talk about what's out there and do they currently use?
    [TestClass]
    public class DeviceControllerTests
    {
        [TestMethod]
        public void TurOnDevices_FromAllDevices_OnlyOnlineShouldBeTurnedOn()
        {
            //Arrange
            var allDevices =
                 new StubDevice []
                {
                    new StubDevice("FoodProcessor", false),
                    new StubDevice("Living Room TV", true),
                    new StubDevice("Washing Machine", false),
                    new StubDevice("HIFI system", true),
                };
            var repo = new FakeDeviceRepository { AllDevices = allDevices };
            var sut = new DeviceController(repo,null,null);

            //Act
            sut.TurnOnDevices();

            //Assert
            Assert.IsTrue(allDevices.Where(d => d.IsOnLine).All(d => d.IsOn));
        }

        [TestMethod]
        public void UpdateFirmWare_FetchLatestFirmware_ApplyToDevice()
        {
            //Arrange
            var deviceFirwareFetcher = Substitute.For<IDeviceFirmwareFetcher>();
            var repository = Substitute.For<IDeviceRepository>();
            var device = Substitute.For<Device>(Guid.NewGuid(), "MyDevice", null,false);
            var newFirmware = new Firmware();
            var dateTime = new DateTime(2019, 8, 6, 11, 0, 0);

            repository.AllDevices.Returns(new[] { device });
            deviceFirwareFetcher.GetLatestFirmWareFor(device).Returns(newFirmware);
            var sut = new DeviceController(repository, deviceFirwareFetcher, new FakeDateTimeProvider { Now = dateTime });

            //Act
            sut.UpdateFirmWare();

            //Assert
            deviceFirwareFetcher.Received(1).GetLatestFirmWareFor(device);
            device.UpdateFirmware(newFirmware, dateTime);
        }

       
        #region nested test doubles

        private class FakeDeviceRepository : IDeviceRepository
        {
            public IEnumerable<Device> AllDevices { get; set; }

            public FakeDeviceRepository()
            {
                AllDevices = new StubDevice[]
              {
                    new StubDevice("FoodProcessor", false),
                    new StubDevice("Living Room TV", true),
                    new StubDevice("Washing Machine", false),
                    new StubDevice("HIFI system", true),
              };
            }
        }

        private class StubDevice : Device
        {
            public bool IsOn { get; private set; }
            public StubDevice(string name, bool IsOnline) : this(Guid.NewGuid(), name, new Firmware(), IsOnline)
            {
            }
            public StubDevice(Guid id, string name, Firmware defaultFirmware, bool activate = false) : base(id, name, defaultFirmware, activate)
            {
            }

            public override void TurnOn()
            {
                IsOn = true;
            }

            public override void UpdateFirmware(Firmware newFirware, DateTime dateTime = default)
            {
                base.UpdateFirmware(newFirware, dateTime);
            }
        }

        #endregion


    }

    public class FakeDeviceFirmWareFetcher : IDeviceFirmwareFetcher
    {
        public Firmware GetLatestFirmWareFor(Device device)
        {
            return new Firmware();
        }
    }
}
