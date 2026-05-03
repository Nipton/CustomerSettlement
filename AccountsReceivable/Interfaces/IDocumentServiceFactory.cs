
using AccountsReceivable.Models;

namespace AccountsReceivable.Interfaces
{
    public interface IDocumentServiceFactory
    {
        IDocumentService<AccountHeader> GetActService();
        IDocumentService<AccountHeader> GetInvoiceService();
        IDocumentService<ReconciliationReport> GetReconciliationService();
    }
}
