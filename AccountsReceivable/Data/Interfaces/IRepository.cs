using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Data.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<int> AddAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task DeleteAsync(List<T> entities);
    }
}
