
using CustomerSettlement.Interfaces;
using CustomerSettlement.Models;

namespace CustomerSettlement.Services
{
    public class DocumentServiceFactory : IDocumentServiceFactory
    {
        private readonly ActDocumentService actService;
        private readonly InvoiceDocumentService invoiceService;
        private readonly ReconciliationDocumentService reconciliationService;

        public DocumentServiceFactory(ActDocumentService actService, InvoiceDocumentService invoiceService, ReconciliationDocumentService reconciliationService)
        {
            this.actService = actService;
            this.invoiceService = invoiceService;
            this.reconciliationService = reconciliationService;
        }

        public IDocumentService<AccountHeader> GetActService() => actService;
        public IDocumentService<AccountHeader> GetInvoiceService() => invoiceService;
        public IDocumentService<ReconciliationReport> GetReconciliationService() => reconciliationService;
    }
}
