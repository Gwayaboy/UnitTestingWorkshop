using System;
using TestDoubles;

namespace UnitTestProject
{
    public class FakeDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now { get; set; }
    }
}