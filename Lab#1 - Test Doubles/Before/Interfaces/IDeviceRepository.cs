using System;
using System.Collections.Generic;
using TestDoubles.Domain;

namespace TestDoubles.Interfaces
{
    public interface IDeviceRepository : IDisposable
    {
        IEnumerable<Device> AllOnlineDevices { get; }
        IEnumerable<Device> AllDevices { get; }

        Device GetDevice(Guid deviceId);

        void AddDevice(Device newDevice);

        void UpdateDevice(Device device);

    }
}