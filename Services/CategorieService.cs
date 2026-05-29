using Microsoft.EntityFrameworkCore;
using GestionStock.Data;
using GestionStock.Interfaces;
using GestionStock.Models;

namespace GestionStock.Services;

public class CategorieService : ICategorieService
{
      private readonly ApplicationDbContext _context;
      private readonly ILogger<CategorieService> _logger;

      public CategorieService(ApplicationDbContext context, ILogger<CategorieService> logger)
      {
            _context = context;
            _logger = logger;
      }

      public async Task<IEnumerable<Categorie>> GetAllAsync()
      {
            return await _context.Categories
                .Include(c => c.Produits)
                .OrderBy(c => c.Nom)
                .ToListAsync();
      }

      public async Task<Categorie?> GetByIdAsync(int id)
      {
            return await _context.Categories
                .Include(c => c.Produits)
                .FirstOrDefaultAsync(c => c.Id == id);
      }

      public async Task<bool> CreateAsync(Categorie categorie)
      {
            try
            {
                  _context.Add(categorie);
                  await _context.SaveChangesAsync();
                  _logger.LogInformation("Catégorie créée : {Nom}", categorie.Nom);
                  return true;
            }
            catch (Exception ex)
            {
                  _logger.LogError(ex, "Erreur lors de la création de la catégorie {Nom}", categorie.Nom);
                  return false;
            }
      }

      public async Task<bool> UpdateAsync(Categorie categorie)
      {
            try
            {
                  _context.Update(categorie);
                  await _context.SaveChangesAsync();
                  _logger.LogInformation("Catégorie modifiée : {Nom}", categorie.Nom);
                  return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                  _logger.LogError(ex, "Erreur de concurrence pour la catégorie {Id}", categorie.Id);
                  return false;
            }
      }

      public async Task<bool> DeleteAsync(int id)
      {
            var categorie = await _context.Categories.FindAsync(id);
            if (categorie == null) return false;

            _context.Categories.Remove(categorie);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Catégorie supprimée : {Id}", id);
            return true;
      }

      public async Task<bool> NomExisteAsync(string nom, int? excludeId = null)
      {
            return await _context.Categories
                .AnyAsync(c => c.Nom == nom &&
                              (excludeId == null || c.Id != excludeId));
      }

      public async Task<bool> HasProduitsAsync(int id)
      {
            return await _context.Produits.AnyAsync(p => p.CategorieId == id);
      }
}