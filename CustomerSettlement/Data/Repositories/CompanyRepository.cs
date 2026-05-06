using CustomerSettlement.Data.Interfaces;
using CustomerSettlement.Exceptions;
using CustomerSettlement.Helpers;
using CustomerSettlement.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerSettlement.Data.Repositories
{
    public class CompanyRepository : NotifiableRepository, ICompanyRepository
    {
        private readonly IDbContextFactory<ApplicationContext> factory;
        public CompanyRepository(IDbContextFactory<ApplicationContext> factory) 
        {
            this.factory = factory;
        }
        public async Task<Company?> GetCompanyAsync(int id)
        {  
            using var context = await factory.CreateDbContextAsync();
            return await context.Companies.FindAsync(id);
        }
        public async Task<int> CreateCompanyAsync(Company company)
        {
            using var context = await factory.CreateDbContextAsync();
            await context.Companies.AddAsync(company);
            await context.SaveChangesAsync();
            await NotifyDataChangedAsync();
            return company.Id;
        }
        public async Task UpdateCompanyAsync(Company company)
        {
            using var context = await factory.CreateDbContextAsync();
            context.Update(company);
            await context.SaveChangesAsync();
            await NotifyDataChangedAsync();
        }
        public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
        {
            using var context = await factory.CreateDbContextAsync();
            return await context.Companies.Where(x => x.Id != Constants.OWN_COMPANY_ID).ToListAsync();           
        }
        public async Task DeleteCompaniesAsync(List<Company> companies)
        {
            if (companies == null || companies.Count == 0) return;
            try
            {
                using var context = await factory.CreateDbContextAsync();
                context.Companies.AttachRange(companies);
                context.Companies.RemoveRange(companies);
                await context.SaveChangesAsync();
                await NotifyDataChangedAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqliteException sqliteEx && sqliteEx.SqliteErrorCode == 19)
                {
                    var message = companies.Count == 1  ? "Невозможно удалить компанию. Есть связанные записи." : "Невозможно удалить компании. Одна или несколько имеют связанные записи.";
                    throw new DeleteRestrictedException(message);
                }
                throw;
            }
        }
    }
}
