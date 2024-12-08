using Microsoft.EntityFrameworkCore;
using Gestion_Stock.Models;

public class AppDbContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Categorie> Categories { get; set; }
    public DbSet<Produit> Produits { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(@"Data Source=../../../GestionStock.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("Clients");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nom)
                  .IsRequired()
                  .HasMaxLength(100); 
            entity.Property(e => e.Prenom)
                  .IsRequired()
                  .HasMaxLength(100);
            entity.Property(e => e.Adresse)
                  .HasMaxLength(255);
        });

        modelBuilder.Entity<Categorie>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nom)
                  .IsRequired()
                  .HasMaxLength(50);
        });

        modelBuilder.Entity<Produit>(entity =>
        {
            entity.ToTable("Produits");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nom)
                  .IsRequired()
                  .HasMaxLength(100);
            entity.Property(e => e.QuantiteProduct)
                  .IsRequired();
            entity.Property(e => e.Prix)
                  .IsRequired()
                  .HasColumnType("decimal(10, 2)"); 
            entity.Property(e => e.Emplacement)
                  .HasMaxLength(50);

            entity.HasOne(e => e.Categorie)
                  .WithMany(c => c.Produits)
                  .HasForeignKey(e => e.CategorieId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuantiteOrder)
                  .IsRequired();
            entity.Property(e => e.DateCommande)
                  .IsRequired();
            entity.Property(e => e.Status)
                  .HasMaxLength(50);

            entity.HasOne(e => e.Product)
                  .WithMany()
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username)
                  .IsRequired()
                  .HasMaxLength(50);
            entity.Property(e => e.PasswordHash)
                  .IsRequired();
            entity.Property(e => e.Role)
                  .HasMaxLength(20);
        });
    }
}
