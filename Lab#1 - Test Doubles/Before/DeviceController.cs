using System;
using TestDoubles.Interfaces;
using TestDoubles.Domain;
using System.Linq;

namespace TestDoubles
{
    public class DeviceController
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IDeviceFirmwareFetcher _deviceFirmwareFetcher;
        private readonly IDateTimeProvider _dateTimeProvider;


        public DeviceController(IDeviceRepository deviceRepository, 
                                IDeviceFirmwareFetcher deviceFirmwareFetcher,
                                IDateTimeProvider dateTimeProvider)
        {
            _deviceRepository = deviceRepository;
            _deviceFirmwareFetcher = deviceFirmwareFetcher;
            _dateTimeProvider = dateTimeProvider;
        }

        public void TurnOnDevices()
        {
            foreach (var device in _deviceRepository.AllDevices.Where(d => d.IsOnLine))
            {
                device.TurnOn();
            }
        }

        public void UpdateFirmWare()
        {
            foreach (var device in _deviceRepository.AllDevices)
            {
                var latestFirmare = _deviceFirmwareFetcher.GetLatestFirmWareFor(device);

                device.UpdateFirmware(latestFirmare, _dateTimeProvider.Now);
            }
        }


    }
}

