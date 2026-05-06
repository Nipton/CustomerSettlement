using CustomerSettlement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerSettlement.Data.Interfaces
{
    public interface INomenclatureRepository
    {
        Task<int> AddAsync(Nomenclature entity);
        Task<IEnumerable<Nomenclature>> GetAllAsync();
        Task DeleteAsync(Nomenclature entity);
        public List<string> GetAllUnits();
        event Func<Task>? DataChanged;
    }
}
