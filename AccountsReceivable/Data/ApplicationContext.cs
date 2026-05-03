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
        public DbSet<AccountHeader> AccountHeaders { get; set; } = null!;
        public DbSet<AccountLine> AccountLines { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
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
            modelBuilder.Entity<Company>().HasData(new Company { Id = -1, Name = "Моя компания" });

            modelBuilder.Entity<Contract>().HasOne(c => c.Company).WithMany(c => c.Contracts).HasForeignKey(c => c.CompanyId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AccountHeader>().HasOne(a => a.Company).WithMany().OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AccountHeader>().HasOne(a => a.OwnerCompany).WithMany().OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AccountHeader>().HasOne(a => a.Contract).WithMany().OnDelete(DeleteBehavior.Restrict);
        }
    }
}
