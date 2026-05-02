using System.Threading.Tasks;

namespace AccountsReceivable.Interfaces
{
    public interface IDocumentService<T> where T : class
    {
        Task<string> BuildHtml(T report);
    }
}
