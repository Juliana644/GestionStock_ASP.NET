using System.ComponentModel.DataAnnotations;

namespace GestionStock.Models;

public class Categorie
{
      public int Id { get; set; }

      [Required(ErrorMessage = "Le nom est obligatoire")]
      [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 100 caractères")]
      [Display(Name = "Nom de la catégorie")]
      public string Nom { get; set; } = string.Empty;

      [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
      public string? Description { get; set; }

      // Propriété de navigation : EF Core charge les produits liés
      public ICollection<Produit> Produits { get; set; } = new List<Produit>();
}