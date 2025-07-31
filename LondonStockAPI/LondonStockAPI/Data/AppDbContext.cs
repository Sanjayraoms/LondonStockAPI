using LondonStockAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace LondonStockAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {
        }
        public DbSet<Trade> Trades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trade>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Trade>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd(); // Identity column

            //Having a non-clustered index on ticker symbol as it helps
            //in faster retrieval
            modelBuilder.Entity<Trade>()
                .HasIndex(e => e.TickerSymbol)
                .IsUnique(false)
                .HasDatabaseName("IX_Trade_TickerSymbol")
                .IsClustered(false);
        }
    }
}
