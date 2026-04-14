using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Exceptions;
using AccountsReceivable.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace AccountsReceivable.Data.Repositories
{
    public class NomenclatureRepository : NotifiableRepository, INomenclatureRepository
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
            await NotifyDataChangedAsync();
            return entity.Id;
        }
        public async Task DeleteAsync(Nomenclature entity)
        {
            try
            {
                using var context = await factory.CreateDbContextAsync();
                context.Nomenclatures.Attach(entity);
                context.Nomenclatures.Remove(entity);
                await context.SaveChangesAsync();
                await NotifyDataChangedAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqliteException sqliteEx && sqliteEx.SqliteErrorCode == 19)
                {
                    var message = "Невозможно удалить услугу. Есть связанные записи.";
                    throw new DeleteRestrictedException(message);
                }
                throw;
            }
        }

        public async Task<IEnumerable<Nomenclature>> GetAllAsync()
        {
            using var context = await factory.CreateDbContextAsync();
            return await context.Nomenclatures.ToListAsync();
        }
        public List<string> GetAllUnits()
        {
            var json = File.ReadAllText("Data/units.json");
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
    }
}
