using Microsoft.EntityFrameworkCore;
using GestionStock.Data;
using GestionStock.Interfaces;
using GestionStock.Models;

namespace GestionStock.Services;

public class ProduitService : IProduitService
{
      private readonly ApplicationDbContext _context;
      private readonly ILogger<ProduitService> _logger;

      // Injection de dépendances : EF Core et Logger injectés par le conteneur DI
      public ProduitService(ApplicationDbContext context, ILogger<ProduitService> logger)
      {
            _context = context;
            _logger = logger;
      }

      public async Task<IEnumerable<Produit>> GetAllAsync(string? recherche, int? categorieId, bool? stockFaible)
      {
            var query = _context.Produits
                .Include(p => p.Categorie)
                .AsQueryable();

            if (!string.IsNullOrEmpty(recherche))
                  query = query.Where(p =>
                      p.Nom.Contains(recherche) ||
                      p.Reference.Contains(recherche));

            if (categorieId.HasValue)
                  query = query.Where(p => p.CategorieId == categorieId.Value);

            if (stockFaible == true)
                  query = query.Where(p => p.QuantiteEnStock <= p.SeuilAlerte);

            return await query.OrderBy(p => p.Nom).ToListAsync();
      }

      public async Task<Produit?> GetByIdAsync(int id)
      {
            return await _context.Produits
                .Include(p => p.Categorie)
                .Include(p => p.MouvementsStock
                    .OrderByDescending(m => m.DateMouvement)
                    .Take(10))
                .FirstOrDefaultAsync(p => p.Id == id);
      }

      public async Task<bool> CreateAsync(Produit produit)
      {
            try
            {
                  produit.DateAjout = DateTime.Now;
                  _context.Add(produit);
                  await _context.SaveChangesAsync();
                  _logger.LogInformation("Produit créé : {Reference}", produit.Reference);
                  return true;
            }
            catch (Exception ex)
            {
                  _logger.LogError(ex, "Erreur lors de la création du produit {Reference}", produit.Reference);
                  return false;
            }
      }

      public async Task<bool> UpdateAsync(Produit produit)
      {
            try
            {
                  _context.Update(produit);
                  await _context.SaveChangesAsync();
                  _logger.LogInformation("Produit modifié : {Reference}", produit.Reference);
                  return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                  _logger.LogError(ex, "Erreur de concurrence pour le produit {Id}", produit.Id);
                  return false;
            }
      }

      public async Task<bool> DeleteAsync(int id)
      {
            var produit = await _context.Produits.FindAsync(id);
            if (produit == null) return false;

            _context.Produits.Remove(produit);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Produit supprimé : {Id}", id);
            return true;
      }

      public async Task<bool> ReferenceExisteAsync(string reference, int? excludeId = null)
      {
            return await _context.Produits
                .AnyAsync(p => p.Reference == reference &&
                              (excludeId == null || p.Id != excludeId));
      }

      public async Task<int> CountStockFaibleAsync()
      {
            return await _context.Produits
                .CountAsync(p => p.QuantiteEnStock <= p.SeuilAlerte && p.Actif);
      }
}