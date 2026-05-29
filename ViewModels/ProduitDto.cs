using System.ComponentModel.DataAnnotations;

namespace GestionStock.ViewModels;

// DTO pour lire un produit (réponse API)
public class ProduitDto
{
      public int Id { get; set; }
      public string Reference { get; set; } = string.Empty;
      public string Nom { get; set; } = string.Empty;
      public string? Description { get; set; }
      public decimal PrixUnitaire { get; set; }
      public int QuantiteEnStock { get; set; }
      public int SeuilAlerte { get; set; }
      public bool Actif { get; set; }
      public DateTime DateAjout { get; set; }
      public int CategorieId { get; set; }
      public string? CategorieNom { get; set; }
      public bool StockCritique => QuantiteEnStock <= SeuilAlerte;
}

// DTO pour créer ou modifier un produit (requête API)
public class ProduitCreateDto
{
      [Required(ErrorMessage = "La référence est obligatoire")]
      [StringLength(50)]
      public string Reference { get; set; } = string.Empty;

      [Required(ErrorMessage = "Le nom est obligatoire")]
      [StringLength(200)]
      public string Nom { get; set; } = string.Empty;

      [StringLength(1000)]
      public string? Description { get; set; }

      [Required]
      [Range(0.01, 9999999.99)]
      public decimal PrixUnitaire { get; set; }

      [Required]
      [Range(0, int.MaxValue)]
      public int QuantiteEnStock { get; set; }

      [Range(0, int.MaxValue)]
      public int SeuilAlerte { get; set; } = 5;

      public bool Actif { get; set; } = true;

      [Required]
      public int CategorieId { get; set; }
}