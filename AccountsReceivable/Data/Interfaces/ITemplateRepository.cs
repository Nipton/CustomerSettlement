using System.Threading.Tasks;

namespace AccountsReceivable.Data.Interfaces
{
    public interface ITemplateRepository
    {
        Task<string> GetTemplateAsync(string templateName);
    }
}
