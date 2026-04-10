using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Data.Repositories
{
    public class CategoryRepository : IRepository<Category>
    {
        private readonly IDbContextFactory<ApplicationContext> factory;
        public CategoryRepository(IDbContextFactory<ApplicationContext> factory)
        {
            this.factory = factory;
        }
        public Task<int> AddAsync(Category entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Category entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            using var context = await factory.CreateDbContextAsync();
            return await context.Category.ToListAsync();
        }
    }
}
