using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionStock.Models;

namespace GestionStock.Controllers;

[Authorize(Roles = "Admin")]  // Tout le contrôleur est protégé
public class AdminController : Controller
{
      private readonly UserManager<AppUser> _userManager;
      private readonly RoleManager<IdentityRole> _roleManager;
      private readonly ILogger<AdminController> _logger;

      public AdminController(
          UserManager<AppUser> userManager,
          RoleManager<IdentityRole> roleManager,
          ILogger<AdminController> logger)
      {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
      }

      // GET /Admin
      public async Task<IActionResult> Index()
      {
            var users = await _userManager.Users.ToListAsync();

            // Pour chaque utilisateur, on récupère ses rôles
            var usersAvecRoles = new List<(AppUser User, IList<string> Roles)>();
            foreach (var user in users)
            {
                  var roles = await _userManager.GetRolesAsync(user);
                  usersAvecRoles.Add((user, roles));
            }

            ViewBag.UsersAvecRoles = usersAvecRoles;
            ViewBag.TotalUsers = users.Count;
            ViewBag.TotalAdmins = usersAvecRoles.Count(u => u.Roles.Contains("Admin"));

            _logger.LogInformation("Admin {Admin} a accédé à la liste des utilisateurs",
                User.Identity?.Name);

            return View();
      }

      // POST /Admin/ChangerRole
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> ChangerRole(string userId, string role)
      {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var rolesActuels = await _userManager.GetRolesAsync(user);

            // Supprimer tous les rôles actuels
            await _userManager.RemoveFromRolesAsync(user, rolesActuels);

            // Assigner le nouveau rôle
            await _userManager.AddToRoleAsync(user, role);

            _logger.LogInformation("Rôle de {User} changé en {Role} par {Admin}",
                user.Email, role, User.Identity?.Name);

            TempData["Succes"] = $"Rôle de {user.Email} changé en {role}.";
            return RedirectToAction(nameof(Index));
      }

      // POST /Admin/ActiverDesactiver
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> ActiverDesactiver(string userId)
      {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.Actif = !user.Actif;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Compte {User} {Statut} par {Admin}",
                user.Email,
                user.Actif ? "activé" : "désactivé",
                User.Identity?.Name);

            TempData["Succes"] = $"Compte {user.Email} {(user.Actif ? "activé" : "désactivé")}.";
            return RedirectToAction(nameof(Index));
      }
}