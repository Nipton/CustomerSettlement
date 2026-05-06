using System.Threading.Tasks;

namespace CustomerSettlement.Data.Interfaces
{
    public interface ITemplateRepository
    {
        Task<string> GetTemplateAsync(string templateName);
    }
}
