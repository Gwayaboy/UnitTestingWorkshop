﻿using System;
using System.Collections.Generic;
using TestDoubles.Domain;

namespace TestDoubles.Interfaces
{
    public interface IDeviceRepository : IDisposable
    {
        IEnumerable<Device> AllDevices { get; }
    }
}