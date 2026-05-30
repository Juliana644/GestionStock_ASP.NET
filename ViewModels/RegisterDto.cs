using System.ComponentModel.DataAnnotations;

namespace GestionStock.ViewModels;

public class RegisterDto
{
      [Required(ErrorMessage = "Le prénom est obligatoire")]
      [StringLength(100)]
      public string Prenom { get; set; } = string.Empty;

      [Required(ErrorMessage = "Le nom est obligatoire")]
      [StringLength(100)]
      public string Nom { get; set; } = string.Empty;

      [Required(ErrorMessage = "L'email est obligatoire")]
      [EmailAddress(ErrorMessage = "Email invalide")]
      public string Email { get; set; } = string.Empty;

      [Required(ErrorMessage = "Le mot de passe est obligatoire")]
      [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
      [DataType(DataType.Password)]
      public string MotDePasse { get; set; } = string.Empty;

      [Required(ErrorMessage = "La confirmation est obligatoire")]
      [DataType(DataType.Password)]
      [Compare("MotDePasse", ErrorMessage = "Les mots de passe ne correspondent pas")]
      public string ConfirmationMotDePasse { get; set; } = string.Empty;
}