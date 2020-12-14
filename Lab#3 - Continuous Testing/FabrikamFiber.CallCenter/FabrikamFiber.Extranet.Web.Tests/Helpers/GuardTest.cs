namespace FabrikamFiber.Extranet.Web.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FabrikamFiber.Extranet.Web.Helpers;
    using Xunit;

    public class GuardTest
    {
        [Xunit.Fact]
        public void ItShouldThrowExceptionIfArgumentIsNull()
        {
            try
            {
                Guard.ThrowIfNull(null, "value");
            }
            catch (ArgumentNullException)
            { }
        }

        [Fact]
        public void ItShouldNotThrowExceptionIfArgumentIsNotNull()
        {
            Guard.ThrowIfNull("this is not null", "value");
        }

        [Fact]
        public void ItShouldThrowExceptionIfArgumentIsNullOrEmpty()
        {
            Assert.Throws<ArgumentNullException>( () => Guard.ThrowIfNullOrEmpty(string.Empty, "value"));
        }

        [Fact]
        public void ItShouldNotThrowExceptionIfArgumentIsNotNullOrEmpty()
        {
            Guard.ThrowIfNullOrEmpty("not null or empty", "value");
        }

        [Fact]
        public void ItShouldThrowExceptionIfArgumentIsNotInRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Guard.ThrowIfNotInRange(1, 2, 3, "value"));
        }

        [Fact]
        public void ItShouldNotThrowExceptionIfArgumentIsNotLesserThanTheMin()
        {
            Guard.ThrowIfNotInRange(2, 1, 3, "value");
        }
    }
}
