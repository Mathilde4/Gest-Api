using GestStockAPI.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<TotalSales> TotalSales { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<Stock> Stocks { get; set; }
     public DbSet<Admin> Admins { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuration du modèle
        modelBuilder.Entity<Product>().ToTable("Products")
                    .HasKey(p => p.ProductId);
                    
        modelBuilder.Entity<Category>().ToTable("Categories")
                    .HasKey(c => c.CategoryId);
        modelBuilder.Entity<User>().ToTable("Users")
                    .HasKey(u => u.UserId);
        modelBuilder.Entity<Admin>().ToTable("Admins")
                    .HasKey(a => a.AdminId);
        modelBuilder.Entity<Supplier>().ToTable("Suppliers")
                    .HasKey(s => s.SupplierId);
        modelBuilder.Entity<Purchase>().ToTable("Purchases")
                    .HasKey(p => p.PurchaseId);
        modelBuilder.Entity<Sale>().ToTable("Sales")
                    .HasKey(s => s.SaleId);
       modelBuilder.Entity<TotalSales>().ToTable("TotalSales")
             .HasKey(s => s.Id); 


    }

}
