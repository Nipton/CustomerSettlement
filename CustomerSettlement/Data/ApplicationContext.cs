using CustomerSettlement.Helpers;
using CustomerSettlement.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerSettlement.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Contract> Contracts { get; set; } = null!;
        public DbSet<Company> Companies { get; set; } = null!;
        public DbSet<AccountHeader> AccountHeaders { get; set; } = null!;
        public DbSet<AccountLine> AccountLines { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<Nomenclature> Nomenclatures { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>().HasData(new Company { Id = Constants.OWN_COMPANY_ID, Name = "Моя компания" });

            modelBuilder.Entity<Contract>().HasOne(c => c.Company).WithMany(c => c.Contracts).HasForeignKey(c => c.CompanyId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AccountHeader>().HasOne(a => a.Company).WithMany().OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AccountHeader>().HasOne(a => a.OwnerCompany).WithMany().OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AccountHeader>().HasOne(a => a.Contract).WithMany().OnDelete(DeleteBehavior.Restrict);
        }
    }
}
