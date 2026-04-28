using AccountsReceivable.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountsReceivable.Data.Interfaces
{
    public interface IContractRepository
    {
        Task<int> AddAsync(Contract entity);
        Task<IEnumerable<Contract>> GetAllAsync();
        Task DeleteAsync(List<Contract> entities);
        Task<IEnumerable<Contract>> GetContractsByCompanyIdAsync(int id);
    }
}
