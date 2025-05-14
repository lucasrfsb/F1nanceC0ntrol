using F1nanceC0ntrol.Models;
using Microsoft.EntityFrameworkCore;

namespace F1nanceC0ntrol.Data
{
    public class FinancialDbContext : DbContext
    {
        public FinancialDbContext(DbContextOptions<FinancialDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        // Novas tabelas para cada tipo de transação
        public DbSet<SellerCommission> SellerCommissions { get; set; }
        public DbSet<DailyOperationCost> DailyOperationCosts { get; set; }
        public DbSet<FixedCost> FixedCosts { get; set; }
        public DbSet<AfterSaleCost> AfterSaleCosts { get; set; }
        public DbSet<CarCost> CarCosts { get; set; }
        public DbSet<FinancingReturn> FinancingReturns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações adicionais se necessário
            modelBuilder.Entity<SellerCommission>().ToTable("SellerCommissions");
            modelBuilder.Entity<DailyOperationCost>().ToTable("DailyOperationCosts");
            modelBuilder.Entity<FixedCost>().ToTable("FixedCosts");
            modelBuilder.Entity<AfterSaleCost>().ToTable("AfterSaleCosts");
            modelBuilder.Entity<CarCost>().ToTable("CarCosts");
            modelBuilder.Entity<FinancingReturn>().ToTable("FinancingReturns");
        }
    }
}