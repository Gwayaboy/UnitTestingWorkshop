using System;

namespace SmartHomeExample.Tests
{
    internal class FakeDateTimeProvider : IDateTimeProvider
    {

        public FakeDateTimeProvider(DateTime dateTime)
        {
            Now = dateTime;
        }

        public FakeDateTimeProvider(int hour, int minute, int seconds) : 
            this(new DateTime(DateTime.Today.Year,
                              DateTime.Today.Month, 
                              DateTime.Today.Day, 
                              hour, 
                              minute, 
                              seconds ))
        {
        }

        public DateTime Now { get; private set; }
    }
}