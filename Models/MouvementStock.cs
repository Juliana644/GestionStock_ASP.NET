using System.ComponentModel.DataAnnotations;

namespace GestionStock.Models;

public enum TypeMouvement
{
      Entree,
      Sortie,
      Ajustement
}

public class MouvementStock
{
      public int Id { get; set; }

      [Required(ErrorMessage = "Le type de mouvement est obligatoire")]
      [Display(Name = "Type de mouvement")]
      public TypeMouvement Type { get; set; }

      [Required(ErrorMessage = "La quantité est obligatoire")]
      [Range(1, int.MaxValue, ErrorMessage = "La quantité doit être au moins 1")]
      [Display(Name = "Quantité")]
      public int Quantite { get; set; }

      [StringLength(500)]
      [Display(Name = "Motif")]
      public string? Motif { get; set; }

      [DataType(DataType.DateTime)]
      [Display(Name = "Date du mouvement")]
      public DateTime DateMouvement { get; set; } = DateTime.Now;

      // Clé étrangère
      [Required]
      public int ProduitId { get; set; }

      // Propriété de navigation
      public Produit? Produit { get; set; }
}