using System;

namespace TestDoubles
{
    public struct DateTimeRange
    {
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        public DateTimeRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }
        public bool Includes(DateTime datetime)
        {
            return datetime >= Start && datetime <= End;
        }
    }
}

