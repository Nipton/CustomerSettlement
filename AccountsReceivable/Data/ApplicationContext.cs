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
        public DbSet<AccountHeader> AccountHeaders { get; set; } = null!;
        public DbSet<AccountLine> AccountLines { get; set; } = null!;
        public DbSet<Payment> Payment { get; set; } = null!;
        public DbSet<Nomenclature> Nomenclatures { get; set; } = null!;

        public ApplicationContext() { }  

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)  
            {
                optionsBuilder.UseSqlite("Data Source=accounts.db");
                optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message));
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountPartOne>().HasData(new AccountPartOne { ID = -1, Company = "" });
            modelBuilder.Entity<Company>().HasData(new Company { Id = -1, Name = "Моя компания" },new Company { Id = 1, Name = "Первая компания" }, new Company { Id = 2, Name = "Вторая" });
            modelBuilder.Entity<Contract>().HasData(new Contract { Id = 1, Number = "54821", CompanyId = 1 }, new Contract { Id = 2, Number = "WE2314", CompanyId = 2 });
            modelBuilder.Entity<Nomenclature>().HasData(new Nomenclature { Id = 1, Name = "Водопровод" });
            modelBuilder.Entity<Nomenclature>().HasData(new Nomenclature { Id = 2, Name = "Водоотведение" });
            modelBuilder.Entity<Nomenclature>().HasData(new Nomenclature { Id = 3, Name = "Уборка улиц" });

            modelBuilder.Entity<Contract>().Property(c => c.Date).HasDefaultValueSql("CURRENT_DATE");
            modelBuilder.Entity<Contract>().HasOne(c => c.Company).WithMany(c => c.Contracts).HasForeignKey(c => c.CompanyId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AccountHeader>().HasOne(a => a.Company).WithMany().OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AccountHeader>().HasOne(a => a.Contract).WithMany().OnDelete(DeleteBehavior.Restrict);
        }
    }
}
