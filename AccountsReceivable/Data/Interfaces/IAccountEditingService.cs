using AccountsReceivable.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Data.Interfaces
{
    public interface IAccountEditingService : IDisposable
    {
        Task<AccountHeader?> GetAccountAsync(int id);
        Task SaveAccountsAsync(AccountHeader accountHeader);
        Task<decimal> GetPaymentSumAsync(int id);
        Task<IEnumerable<Nomenclature>> GetNomenclaturesAsync();
    }
}
