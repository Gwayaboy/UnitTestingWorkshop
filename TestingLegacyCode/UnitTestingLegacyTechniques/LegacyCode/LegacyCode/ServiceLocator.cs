using LegacyCode.Interfaces;

namespace LegacyCode
{
    public class ServiceLocator
    {
        public static IEmailService EmailService { get; internal set; }
        public static IInvoiceService InvoiceService { get; internal set; }
        public static IOrderRepository OrderRepository { get; internal set; }
    }
}