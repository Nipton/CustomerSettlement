using AccountsReceivable.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Data.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company?> GetCompanyAsync(int id);
        Task<int> AddCompanyAsync(Company company);
        Task UpdateCompanyAsync(Company company);
        Task<IEnumerable<Company>> GetAllCounterpartiesAsync();
        Task RemoveCounterpartiesAsync(List<Company> companies);
    }
}
