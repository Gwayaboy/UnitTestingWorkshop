using System;
using TestDoubles.Interfaces;
using TestDoubles.Domain;

namespace TestDoubles
{
    public class DeviceController
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IDeviceSwitcher _deviceSwitcher;
        private readonly IDeviceFirmwareFetcher _deviceFirmwareFetcher;

        public DeviceController(IDeviceRepository deviceRepository, IDeviceSwitcher deviceSwitcher, IDeviceFirmwareFetcher deviceFirmwareFetcher)
        {
            _deviceRepository = deviceRepository;
            _deviceSwitcher = deviceSwitcher;
            _deviceFirmwareFetcher = deviceFirmwareFetcher;
        }

        public void TurnOnDevices(bool includeOfflineDevices = false)
        {
            foreach (var device in _deviceRepository.AllDevices)
            {
                if (includeOfflineDevices && !device.IsOnLine)
                {
                    device.SetOnLine(true);
                }

                if (device.IsOnLine)
                {
                    _deviceSwitcher.TurnOn(device);
                }
            }
        }

        public void UpdateAllDeviceFirmWare(DateTime scheduledDateTime)
        {
            foreach (var device in _deviceRepository.AllDevices)
            {
                var latestFirmare = _deviceFirmwareFetcher.GetLatestFirmWareFor(device);
                var deviceInitiallyOnline = device.IsOnLine;

                if (deviceInitiallyOnline) device.SetOnLine(false);

                device.UpdateFirmware(scheduledDateTime, latestFirmare);

                if (deviceInitiallyOnline) device.SetOnLine(true);

            }
        }

    }
}

