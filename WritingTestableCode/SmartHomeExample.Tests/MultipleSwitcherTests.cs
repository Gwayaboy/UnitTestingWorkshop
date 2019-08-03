using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SmartHomeExample.Tests
{
    [TestClass]
    public class MultipleSwitcherTests
    {
        class FakeDeviceSwitcher : IDeviceSwitcher
        {
            public bool SwitchedOff { get; private set; } = true;

            public void TurnOff()
            {
                SwitchedOff = true;
            }

            public void TurnOn()
            {
                SwitchedOff = false;
            }
        }

        class HeatingSwitcher : FakeDeviceSwitcher { }
        class InsideLightSwitcher : FakeDeviceSwitcher { }

        [TestMethod]
        public void CanSwitchOffAllLightsAndHeating()
        {
            //Arrange
            var allDeviceSwitchers = new FakeDeviceSwitcher[]
            {
                new HeatingSwitcher(),
                new InsideLightSwitcher()
            };

            var sut = new MultipleSwitcher(allDeviceSwitchers);

            //Act
            sut.TurnOff();

            //Assert
            Assert.IsTrue(allDeviceSwitchers.All(d => d.SwitchedOff));
        }
    }

}
