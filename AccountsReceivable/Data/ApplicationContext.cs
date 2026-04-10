using AccountsReceivable.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AccountsReceivable.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Contract> Contracts { get; set; } = null!;
        public DbSet<Company> Companies { get; set; } = null!;
        public DbSet<AccountPartOne> AccountsPartOne { get; set; } = null!;
        public DbSet<AccountPartTwo> AccountsPartTwo { get; set; } = null!;
        public DbSet<Payment> Payment { get; set; } = null!;
        public DbSet<Category> Category { get; set; } = null!;
        public DbSet<Nomenclature> Nomenclatures { get; set; } = null!;

        public ApplicationContext() { }  

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)  
            {
                optionsBuilder.UseSqlite("Data Source=accounts.db");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountPartOne>().HasData(new AccountPartOne { ID = -1, Company = "" });
            modelBuilder.Entity<Company>().HasData(new Company { Id = -1, Name = "Моя компания" });
        }
    }
}
