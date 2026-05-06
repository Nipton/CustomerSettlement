using CustomerSettlement.Data.Interfaces;
using CustomerSettlement.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerSettlement.Data
{
    public class AccountEditingService : IAccountEditingService
    {
        private readonly ApplicationContext context;
        public AccountEditingService(IDbContextFactory<ApplicationContext> factory)
        {
            context = factory.CreateDbContext();
        }
        public async Task<AccountHeader?> GetAccountAsync(int id)
        {
            return await context.AccountHeaders.Include(x => x.AccountsList).FirstOrDefaultAsync(x=>x.Id == id);
        }
        public async Task SaveAccountsAsync(AccountHeader accountHeader)
        {
            if (accountHeader.Id == 0)
                await context.AccountHeaders.AddAsync(accountHeader);
            await context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Nomenclature>> GetNomenclaturesAsync()
        {
            return await context.Nomenclatures.ToListAsync();
        }
        public void Dispose()
        {
            context.Dispose();
        }
    }
}
