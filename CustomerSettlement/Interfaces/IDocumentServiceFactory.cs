
using CustomerSettlement.Models;

namespace CustomerSettlement.Interfaces
{
    public interface IDocumentServiceFactory
    {
        IDocumentService<AccountHeader> GetActService();
        IDocumentService<AccountHeader> GetInvoiceService();
        IDocumentService<ReconciliationReport> GetReconciliationService();
    }
}
