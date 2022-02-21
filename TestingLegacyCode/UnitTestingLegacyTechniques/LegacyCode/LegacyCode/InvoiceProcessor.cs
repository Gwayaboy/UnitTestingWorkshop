using System;

namespace LegacyCode
{
    public class InvoiceProcessor
    {
        public void SendInvoiceLegacy()
        {
            
            
            //Code to get invoice data
            #region do some more stuff with the invoice data
            #endregion
            
            //Code to send invoice by email
            #region do some prep work before sending the invoice
            #endregion
            


            // Now let's add to that some code for logging the invoice
            LoggingInvoice();
        }
        
        public virtual void SendInvoiceRefactored()
        {
            var invoice = PrepareInvoice();
            SendInvoiceByEmail(invoice);

            LoggingInvoice(invoice);

        }

        public virtual void LoggingInvoice(object invoice)
        {

        }

        private virtual void SendInvoiceByEmail(object invoice)
        {
            //Code to send invoice by email
            #region do some prep work before sending the invoice
            #endregion
            ServiceLocator.EmailService.SendInvoice(invoice);
        }

        private static object PrepareInvoice()
        {
            //Code to get invoice data
            return ServiceLocator.InvoiceService.GetInvoice();
            #region do some more stuff with the invoice data
            #endregion
        }
    }
}
