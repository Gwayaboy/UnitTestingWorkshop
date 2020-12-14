namespace FabrikamFiber.Web.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FabrikamFiber.Web.Helpers;
    using Xunit;
    using Guard = Web.Helpers.Guard;

    public class GuardTest
    {
        [Fact]
        public void ItShouldThrowExceptionIfArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => Guard.ThrowIfNull(null, "value"));
        }

        [Fact]
        public void ItShouldNotThrowExceptionIfArgumentIsNotNull()
        {
            Guard.ThrowIfNull("this is not null", "value");
        }

        [Fact]
        public void ItShouldThrowExceptionIfArgumentIsNullOrEmpty()
        {

            Assert.Throws<ArgumentNullException>(() => Guard.ThrowIfNullOrEmpty(string.Empty, "value"));
        }

        [Fact]
        public void ItShouldNotThrowExceptionIfArgumentIsNotNullOrEmpty()
        {
            Guard.ThrowIfNullOrEmpty("not null or empty", "value");
        }

        [Fact]
        public void ItShouldThrowExceptionIfArgumentIsLesserThanZero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Guard.ThrowIfLesserThanZero(-1, "value"));
        }

        [Fact]
        public void ItShouldNotThrowExceptionIfArgumentIsNotLesserThanZero()
        {
            Guard.ThrowIfLesserThanZero(1, "value");
        }
    }
}
