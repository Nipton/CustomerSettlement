using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AccountsReceivable.Data.Repositories
{
    public class NomenclatureRepository : INomenclatureRepository
    {
        private readonly IDbContextFactory<ApplicationContext> factory;
        public NomenclatureRepository(IDbContextFactory<ApplicationContext> factory) 
        {
            this.factory = factory;
        }
        public async Task<int> AddAsync(Nomenclature entity)
        {
            using var context = await factory.CreateDbContextAsync();
            await context.Nomenclatures.AddAsync(entity);
            await context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(Nomenclature entity)
        {
            using var context = await factory.CreateDbContextAsync();
            context.Nomenclatures.Attach(entity);
            context.Nomenclatures.Remove(entity);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Nomenclature>> GetAllAsync()
        {
            using var context = await factory.CreateDbContextAsync();
            return await context.Nomenclatures.ToListAsync();
        }
        public List<string> GetAllUnits()
        {
            var json = File.ReadAllText("units.json");
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
    }
}
