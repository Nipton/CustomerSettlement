using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Models;
using AccountsReceivable.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Data.Repositories
{
    public class CompanyRepository : ICompanyRepository
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
        public async Task<int> AddCompanyAsync(Company company)
        {
            using var context = await factory.CreateDbContextAsync();
            await context.Companies.AddAsync(company);
            await context.SaveChangesAsync();
            return company.Id;
        }
        public async Task UpdateCompanyAsync(Company company)
        {
            using var context = await factory.CreateDbContextAsync();
            context.Update(company);
            await context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Company>> GetAllCounterpartiesAsync()
        {
            using var context = await factory.CreateDbContextAsync();
            return await context.Companies.Where(x => x.Id != Constants.OWN_COMPANY_ID).Include(c => c.Category).ToListAsync();           
        }
        public async Task RemoveCounterpartiesAsync(List<Company> companies)
        {
            using var context = await factory.CreateDbContextAsync();
            context.Companies.AttachRange(companies);
            context.Companies.RemoveRange(companies);
            await context.SaveChangesAsync();
        }
    }
}
