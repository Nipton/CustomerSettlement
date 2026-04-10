using AccountsReceivable.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Data.Interfaces
{
    public interface INomenclatureRepository : IRepository<Nomenclature>
    {
        public List<string> GetAllUnits();
    }
}
