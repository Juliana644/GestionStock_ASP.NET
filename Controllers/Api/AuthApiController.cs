using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestionStock.Models;
using GestionStock.ViewModels;

namespace GestionStock.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class AuthApiController : ControllerBase
{
      private readonly UserManager<AppUser> _userManager;
      private readonly SignInManager<AppUser> _signInManager;
      private readonly IConfiguration _configuration;

      // UserManager : gère les utilisateurs (création, recherche, rôles...)
      // SignInManager : gère la connexion/déconnexion
      // IConfiguration : lit appsettings.json (clé JWT)
      public AuthApiController(
          UserManager<AppUser> userManager,
          SignInManager<AppUser> signInManager,
          IConfiguration configuration)
      {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
      }

      // POST /api/auth/register
      [HttpPost("register")]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<IActionResult> Register([FromBody] RegisterDto dto)
      {
            // Vérifier si l'email existe déjà
            var existant = await _userManager.FindByEmailAsync(dto.Email);
            if (existant != null)
                  return BadRequest(new { message = "Cet email est déjà utilisé." });

            var user = new AppUser
            {
                  UserName = dto.Email,
                  Email = dto.Email,
                  Prenom = dto.Prenom,
                  Nom = dto.Nom,
                  DateInscription = DateTime.Now
            };

            // CreateAsync hache automatiquement le mot de passe
            var result = await _userManager.CreateAsync(user, dto.MotDePasse);

            if (!result.Succeeded)
            {
                  var erreurs = result.Errors.Select(e => e.Description);
                  return BadRequest(new { message = "Erreur lors de la création du compte.", erreurs });
            }

            // Assigner le rôle par défaut
            await _userManager.AddToRoleAsync(user, "Employe");

            return Ok(new { message = "Compte créé avec succès." });
      }

      // POST /api/auth/login
      [HttpPost("login")]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status401Unauthorized)]
      public async Task<IActionResult> Login([FromBody] LoginDto dto)
      {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !user.Actif)
                  return Unauthorized(new { message = "Email ou mot de passe incorrect." });

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.MotDePasse, false);
            if (!result.Succeeded)
                  return Unauthorized(new { message = "Email ou mot de passe incorrect." });

            // Générer le token JWT
            var token = await GenererTokenJwt(user);

            return Ok(new
            {
                  token = token,
                  email = user.Email,
                  prenom = user.Prenom,
                  nom = user.Nom,
                  expiration = DateTime.UtcNow.AddHours(24)
            });
      }

      // Méthode privée : génération du token JWT
      private async Task<string> GenererTokenJwt(AppUser user)
      {
            var roles = await _userManager.GetRolesAsync(user);

            // Claims : informations embarquées dans le token
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, $"{user.Prenom} {user.Nom}"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            // Ajouter les rôles comme claims
            foreach (var role in roles)
                  claims.Add(new Claim(ClaimTypes.Role, role));

            // Clé secrète depuis appsettings.json
            var cle = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Cle"]!));

            var credentials = new SigningCredentials(cle, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
      }
}