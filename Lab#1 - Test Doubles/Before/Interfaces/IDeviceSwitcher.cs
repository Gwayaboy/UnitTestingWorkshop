using TestDoubles.Domain;

namespace TestDoubles.Interfaces
{
    public interface IDeviceSwitcher
    {
        void TurnOn(Device device);
        void TurnOff(Device device);
    }
}
