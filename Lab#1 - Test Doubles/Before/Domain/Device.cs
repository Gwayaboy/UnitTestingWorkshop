using System;

namespace TestDoubles.Domain
{
    public class Device
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        public bool IsOnLine { get; private set; }

        public Firmware CurrentFirmware { get; private set; }

        public DateTimeRange ScheduledFirmwareDateTimeRange { get; }

        public Device(Guid id, string name, Firmware defaultFirmware, bool activate = false)
        {
            Id = id;
            Name = name;
            SetOnLine(activate);
            CurrentFirmware = defaultFirmware;
        }

        public void SetOnLine(bool value)
        {
            IsOnLine = value;
        }

        /**
         * Can only update offline devices for scheduled window
         */
        public void UpdateFirmware(DateTime scheduledUpdate, Firmware newFirware)
        {
            if (!IsOnLine && ScheduledFirmwareDateTimeRange.Includes(scheduledUpdate))
            {
                CurrentFirmware = newFirware;
            }
        }
    }
}

