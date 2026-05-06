using CustomerSettlement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerSettlement.Data.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company?> GetCompanyAsync(int id);
        Task<int> CreateCompanyAsync(Company company);
        Task UpdateCompanyAsync(Company company);
        Task<IEnumerable<Company>> GetAllCompaniesAsync();
        Task DeleteCompaniesAsync(List<Company> companies);
        event Func<Task>? DataChanged;
    }
}
