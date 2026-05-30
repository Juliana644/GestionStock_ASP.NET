using Microsoft.AspNetCore.Identity;

namespace GestionStock.Models;

public class AppUser : IdentityUser
{
      // Champs supplémentaires par rapport à IdentityUser
      public string Prenom { get; set; } = string.Empty;
      public string Nom { get; set; } = string.Empty;
      public DateTime DateInscription { get; set; } = DateTime.Now;
      public bool Actif { get; set; } = true;
}