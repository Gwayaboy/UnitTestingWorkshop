using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegacyCode.Domain;
using Moq;

namespace LegacyCode.Tests
{
    public class Tests
    {
        public void _()
        {
            var sut = new Mock<OrderProcessor>();

            //sut.Setup(s => s.GetOrderChanges()).
        }
    }
    public class TestableOrderProcessor : OrderProcessor
    {

        public override void SaveOrderToFile(Order order)
        {
        }
    }
}
