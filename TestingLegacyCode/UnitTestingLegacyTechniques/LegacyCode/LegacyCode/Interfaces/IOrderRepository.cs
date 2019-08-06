using LegacyCode.Domain;

namespace LegacyCode.Interfaces
{
    public interface IOrderRepository
    {
        Order GetOrderById(int orderId);
    }
}