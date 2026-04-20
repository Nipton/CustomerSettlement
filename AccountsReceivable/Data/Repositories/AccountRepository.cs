using AccountsReceivable.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountsReceivable.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbContextFactory<ApplicationContext> factory;
        public AccountRepository(IDbContextFactory<ApplicationContext> factory)
        {
            this.factory = factory;               
        }
    }
}
