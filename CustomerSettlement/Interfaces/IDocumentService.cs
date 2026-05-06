using System.Threading.Tasks;

namespace CustomerSettlement.Interfaces
{
    public interface IDocumentService<T> where T : class
    {
        Task<string> BuildHtml(T report);
    }
}
