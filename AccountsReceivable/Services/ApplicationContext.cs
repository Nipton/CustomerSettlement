using AccountsReceivable.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AccountsReceivable.Services
{
    internal class ApplicationContext : DbContext
    {
        public DbSet<Contract> Contracts { get; set; } = null!;
        public DbSet<AccountPartOne> AccountsPartOne { get; set; } = null!;
        public DbSet<AccountPartTwo> AccountsPartTwo { get; set; } = null!;
        public DbSet<Payment> Payment { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=accounts.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountPartOne>().HasData(new AccountPartOne { ID = -1, Company = "" });
        }
    }
}
