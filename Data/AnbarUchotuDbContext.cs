using AnbarUchotu.Models;
using Microsoft.EntityFrameworkCore;

namespace AnbarUchotu.Data
{
    public class AnbarUchotuDbContext : DbContext
    {
        public AnbarUchotuDbContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Transaction>()
                .HasMany(t => t.Content)
                .WithOne(sp => sp.Transaction)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<SoldProduct>()
                .HasOne(s => s.Product)
                .WithMany(p => p.SoldProducts);

            builder.Entity<Product>()
                .HasMany(p => p.SoldProducts)
                .WithOne(s => s.Product)
                .OnDelete(DeleteBehavior.SetNull);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SoldProduct> SoldProducts { get; set; }
    }
}