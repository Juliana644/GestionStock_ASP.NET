using System.ComponentModel.DataAnnotations;

namespace GestionStock.ViewModels;

public class LoginDto
{
      [Required(ErrorMessage = "L'email est obligatoire")]
      [EmailAddress(ErrorMessage = "Email invalide")]
      public string Email { get; set; } = string.Empty;

      [Required(ErrorMessage = "Le mot de passe est obligatoire")]
      [DataType(DataType.Password)]
      public string MotDePasse { get; set; } = string.Empty;

      public bool Sesouvenir { get; set; } = false;
}