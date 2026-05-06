using CustomerSettlement.Data.Interfaces;
using CustomerSettlement.Exceptions;
using CustomerSettlement.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CustomerSettlement.Data.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly IDbContextFactory<ApplicationContext> factory;
        public ContractRepository(IDbContextFactory<ApplicationContext> dbContextFactory) 
        { 
            factory = dbContextFactory;
        }
        public async Task<int> AddAsync(Contract entity)
        {
            using var context = await factory.CreateDbContextAsync();
            await context.Contracts.AddAsync(entity);
            await context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(List<Contract> entities)
        {
            try
            {
                using var context = await factory.CreateDbContextAsync();
                context.Contracts.AttachRange(entities);
                context.Contracts.RemoveRange(entities);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqliteException sqliteEx && sqliteEx.SqliteErrorCode == 19)
                {
                    var message = entities.Count == 1 ? "Невозможно удалить контракт. Есть связанные записи." : "Невозможно удалить контракты. Одна или несколько имеют связанные записи.";
                    throw new DeleteRestrictedException(message);
                }
                throw;
            }
        }

        public async Task<IEnumerable<Contract>> GetAllAsync()
        {
            using var context = await factory.CreateDbContextAsync();
            return await context.Contracts.Include(c => c.Company).ToListAsync();
        }
        public async Task<IEnumerable<Contract>> GetContractsByCompanyIdAsync(int id)
        {
            using var context = await factory.CreateDbContextAsync();
            return await context.Contracts.Where(x => x.CompanyId == id).ToListAsync();
        }
    }
}