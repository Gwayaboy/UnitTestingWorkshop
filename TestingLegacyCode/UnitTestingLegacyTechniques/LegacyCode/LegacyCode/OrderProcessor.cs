using LegacyCode.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LegacyCode
{
    public class OrderProcessor
    {
        public void SaveOrder(int orderId)
        {
            var order = ServiceLocator.OrderRepository.GetOrderById(orderId);
            GetOrderChanges();
            SaveOrderToFile(order);
        }

        public virtual void SaveOrderToFile(Order order)
        {
            #region Save order to file system
            #endregion
        }

        internal void GetOrderChanges()
        {
            #region Get order changes stuff
            #endregion
        }
    }
}
