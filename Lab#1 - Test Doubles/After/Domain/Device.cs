using SmartHomeExample;
using System;

namespace TestDoubles.Domain
{
    public class Device
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        public bool IsOnLine { get; private set; }

        public Firmware CurrentFirmware { get; private set; }

        public TimeOfDay ScheduledTimeOfDay { get; }

        public Device(Guid id, string name, Firmware defaultFirmware, bool activate = false)
        {
            Id = id;
            Name = name;
            SetOnLine(activate);
            CurrentFirmware = defaultFirmware;
        }

        public virtual void SetOnLine(bool value)
        {
            IsOnLine = value;
        }

        public virtual void TurnOn()
        {
            // Do some complicated call to the physical device. 
        }

        /**
         * Can only update offline devices for scheduled window
         */
        public virtual void UpdateFirmware(Firmware newFirware, DateTime dateTime = default)
        {
            if (!IsOnLine)
            {
                CurrentFirmware = newFirware;
            }
            else if (ScheduledTimeOfDay == GetTimeOfDay(dateTime))
            {
                SetOnLine(false);
                CurrentFirmware = newFirware;
                SetOnLine(true);
            }
        }

        public TimeOfDay GetTimeOfDay(DateTime dateTime)
        {
            if (dateTime.Hour >= 0 && dateTime.Hour < 6)
            {
                return TimeOfDay.Night;
            }
            if (dateTime.Hour >= 6 && dateTime.Hour < 12)
            {
                return TimeOfDay.Morning;
            }
            if (dateTime.Hour >= 12 && dateTime.Hour < 18)
            {
                return TimeOfDay.Afternoon;
            }
            return TimeOfDay.Evening;
        }

    }
}

