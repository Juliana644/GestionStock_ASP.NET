using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GestionStock.Models;

namespace GestionStock.Data;

// IdentityDbContext<AppUser> remplace DbContext
// Il ajoute automatiquement les tables Identity
public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Produit> Produits { get; set; }
    public DbSet<Categorie> Categories { get; set; }
    public DbSet<MouvementStock> MouvementsStock { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Important : appelle la config Identity

        modelBuilder.Entity<Produit>()
            .HasOne(p => p.Categorie)
            .WithMany(c => c.Produits)
            .HasForeignKey(p => p.CategorieId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MouvementStock>()
            .HasOne(m => m.Produit)
            .WithMany(p => p.MouvementsStock)
            .HasForeignKey(m => m.ProduitId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Produit>()
            .HasIndex(p => p.Reference)
            .IsUnique();

        // Seed data
        modelBuilder.Entity<Categorie>().HasData(
            new Categorie { Id = 1, Nom = "Ordinateurs", Description = "PC portables et desktops" },
            new Categorie { Id = 2, Nom = "Périphériques", Description = "Souris, claviers, écrans..." },
            new Categorie { Id = 3, Nom = "Composants", Description = "RAM, disques durs, processeurs..." },
            new Categorie { Id = 4, Nom = "Réseaux", Description = "Routeurs, switches, câbles..." }
        );

        modelBuilder.Entity<Produit>().HasData(
            new Produit
            {
                Id = 1,
                Reference = "PC-001",
                Nom = "Laptop Dell Inspiron 15",
                Description = "Intel Core i5, 8GB RAM, 256GB SSD",
                PrixUnitaire = 650000,
                QuantiteEnStock = 10,
                SeuilAlerte = 3,
                CategorieId = 1,
                DateAjout = new DateTime(2025, 1, 1)
            },
            new Produit
            {
                Id = 2,
                Reference = "PER-001",
                Nom = "Souris Logitech M100",
                Description = "Souris filaire USB",
                PrixUnitaire = 8000,
                QuantiteEnStock = 50,
                SeuilAlerte = 10,
                CategorieId = 2,
                DateAjout = new DateTime(2025, 1, 1)
            },
            new Produit
            {
                Id = 3,
                Reference = "COM-001",
                Nom = "RAM Kingston 8GB DDR4",
                Description = "Mémoire DDR4 2666MHz",
                PrixUnitaire = 25000,
                QuantiteEnStock = 30,
                SeuilAlerte = 5,
                CategorieId = 3,
                DateAjout = new DateTime(2025, 1, 1)
            }
        );
    }
}