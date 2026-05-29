
using GestionStock.Models;

namespace GestionStock.Interfaces;

public interface IProduitService
{
      Task<IEnumerable<Produit>> GetAllAsync(string? recherche, int? categorieId, bool? stockFaible);
      Task<Produit?> GetByIdAsync(int id);
      Task<bool> CreateAsync(Produit produit);
      Task<bool> UpdateAsync(Produit produit);
      Task<bool> DeleteAsync(int id);
      Task<bool> ReferenceExisteAsync(string reference, int? excludeId = null);
      Task<int> CountStockFaibleAsync();
}