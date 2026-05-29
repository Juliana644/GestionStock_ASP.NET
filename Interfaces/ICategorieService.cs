using GestionStock.Models;

namespace GestionStock.Interfaces;

public interface ICategorieService
{
      Task<IEnumerable<Categorie>> GetAllAsync();
      Task<Categorie?> GetByIdAsync(int id);
      Task<bool> CreateAsync(Categorie categorie);
      Task<bool> UpdateAsync(Categorie categorie);
      Task<bool> DeleteAsync(int id);
      Task<bool> NomExisteAsync(string nom, int? excludeId = null);
      Task<bool> HasProduitsAsync(int id);
}