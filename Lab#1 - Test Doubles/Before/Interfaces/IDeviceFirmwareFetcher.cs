using System;
using TestDoubles.Domain;

namespace TestDoubles.Interfaces
{
    public interface IDeviceFirmwareFetcher
    {
        Firmware GetLatestFirmWareFor(Device device);
    }
}