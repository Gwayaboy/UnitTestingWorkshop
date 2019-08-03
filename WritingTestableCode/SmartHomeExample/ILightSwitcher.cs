using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartHomeExample
{
    /**
     * Composition over inheritance
     */

    public interface IDeviceSwitcher
    {
        void TurnOn();
        void TurnOff();
    }

    public interface ILightSwitcher : IDeviceSwitcher
    {
    }


    public class FrontDoorLightSwitcher : ILightSwitcher
    {
        public void TurnOff()
        {
            throw new NotImplementedException();
        }

        public void TurnOn()
        {
            throw new NotImplementedException();
        }
    }

    public class HeatingSwitcher : IDeviceSwitcher
    {
        public void TurnOff()
        {
            throw new NotImplementedException();
        }

        public void TurnOn()
        {
            throw new NotImplementedException();
        }
    }

    public class MultipleSwitcher : IDeviceSwitcher
    {
        private readonly IDeviceSwitcher[] _switchers;

        public MultipleSwitcher(params IDeviceSwitcher[] switchers)
        {
            _switchers = switchers;
        }
        public void TurnOff()
        {
            foreach (var s in _switchers) s.TurnOff();
        }

        public void TurnOn()
        {
            foreach (var s in _switchers) s.TurnOn();
        }

    }
}
