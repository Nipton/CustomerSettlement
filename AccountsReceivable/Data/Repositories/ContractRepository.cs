using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AccountsReceivable.Data.Repositories
{
    public class ContractRepository : IRepository<Contract>
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
            using var context = await factory.CreateDbContextAsync();
            context.Contracts.AttachRange(entities);
            context.Contracts.RemoveRange(entities);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Contract>> GetAllAsync()
        {
            using var context = await factory.CreateDbContextAsync();
            return await context.Contracts.Include(c => c.Company).Include(c => c.Nomenclature).ToListAsync();
        }
    }
}
