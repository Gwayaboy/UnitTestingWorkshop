using System;
using System.Reflection.Metadata.Ecma335;

namespace SmartHomeExample
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }

    public class DefaultDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}