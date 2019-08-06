using System;

namespace LegacyCode
{
    public class InvoiceProcessor
    {
        public void SendInvoice()
        {
            DoStuffForInvoices();
            Logging();
        }

        public void Logging()
        {
            throw new NotImplementedException();
        }

        private static void DoStuffForInvoices()
        {
            //Code to get invoice data
            var invoice = ServiceLocator.InvoiceService.GetInvoice();
            #region do some more stuff with the invoice data
            #endregion

            //Code to send invoice by email
            #region do some prep work before sending the invoice
            #endregion
            ServiceLocator.EmailService.SendInvoice(invoice);
        }
    }
}
