using System.ComponentModel.DataAnnotations;

namespace GestionStock.ViewModels;

// DTO pour lire une catégorie
public class CategorieDto
{
      public int Id { get; set; }
      public string Nom { get; set; } = string.Empty;
      public string? Description { get; set; }
      public int NombreProduits { get; set; }
}

// DTO pour créer ou modifier une catégorie
public class CategorieCreateDto
{
      [Required(ErrorMessage = "Le nom est obligatoire")]
      [StringLength(100, MinimumLength = 2)]
      public string Nom { get; set; } = string.Empty;

      [StringLength(500)]
      public string? Description { get; set; }
}