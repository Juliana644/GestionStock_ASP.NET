using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionStock.Models;

public class Produit
{
      public int Id { get; set; }

      [Required(ErrorMessage = "La référence est obligatoire")]
      [StringLength(50, ErrorMessage = "La référence ne peut pas dépasser 50 caractères")]
      [Display(Name = "Référence")]
      public string Reference { get; set; } = string.Empty;

      [Required(ErrorMessage = "Le nom est obligatoire")]
      [StringLength(200, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 200 caractères")]
      [Display(Name = "Nom du produit")]
      public string Nom { get; set; } = string.Empty;

      [StringLength(1000)]
      public string? Description { get; set; }

      [Required(ErrorMessage = "Le prix est obligatoire")]
      [Range(0.01, 9999999.99, ErrorMessage = "Le prix doit être supérieur à 0")]
      [DataType(DataType.Currency)]
      [Column(TypeName = "decimal(18,2)")]
      [Display(Name = "Prix unitaire")]
      public decimal PrixUnitaire { get; set; }

      [Required(ErrorMessage = "La quantité en stock est obligatoire")]
      [Range(0, int.MaxValue, ErrorMessage = "La quantité ne peut pas être négative")]
      [Display(Name = "Quantité en stock")]
      public int QuantiteEnStock { get; set; }

      [Range(0, int.MaxValue)]
      [Display(Name = "Seuil d'alerte")]
      public int SeuilAlerte { get; set; } = 5;

      [DataType(DataType.Date)]
      [Display(Name = "Date d'ajout")]
      public DateTime DateAjout { get; set; } = DateTime.Now;

      [Display(Name = "Actif")]
      public bool Actif { get; set; } = true;

      // Clé étrangère vers Categorie
      [Required(ErrorMessage = "La catégorie est obligatoire")]
      [Display(Name = "Catégorie")]
      public int CategorieId { get; set; }

      // Propriétés de navigation
      public Categorie? Categorie { get; set; }
      public ICollection<MouvementStock> MouvementsStock { get; set; } = new List<MouvementStock>();
}