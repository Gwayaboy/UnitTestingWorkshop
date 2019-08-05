using System;

namespace TestDoubles
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}