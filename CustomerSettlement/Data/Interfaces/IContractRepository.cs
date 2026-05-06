using CustomerSettlement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerSettlement.Data.Interfaces
{
    public interface IContractRepository
    {
        Task<int> AddAsync(Contract entity);
        Task<IEnumerable<Contract>> GetAllAsync();
        Task DeleteAsync(List<Contract> entities);
        Task<IEnumerable<Contract>> GetContractsByCompanyIdAsync(int id);
    }
}
