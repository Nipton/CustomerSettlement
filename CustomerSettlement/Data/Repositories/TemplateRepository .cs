using CustomerSettlement.Data.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace CustomerSettlement.Data.Repositories
{
    public class TemplateRepository : ITemplateRepository
    {
        public async Task<string> GetTemplateAsync(string templateName)
        {
            string path = Path.Combine("Templates", templateName);
            return await File.ReadAllTextAsync(path);
        }
    }
}
