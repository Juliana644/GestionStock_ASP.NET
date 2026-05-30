using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GestionStock.Models;
using GestionStock.ViewModels;

namespace GestionStock.Controllers;

public class AccountController : Controller
{
      private readonly UserManager<AppUser> _userManager;
      private readonly SignInManager<AppUser> _signInManager;

      public AccountController(
          UserManager<AppUser> userManager,
          SignInManager<AppUser> signInManager)
      {
            _userManager = userManager;
            _signInManager = signInManager;
      }

      // GET /Account/Login
      public IActionResult Login(string? returnUrl = null)
      {
            ViewBag.ReturnUrl = returnUrl;
            return View();
      }

      // POST /Account/Login
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Login(LoginDto dto, string? returnUrl = null)
      {
            if (!ModelState.IsValid) return View(dto);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !user.Actif)
            {
                  ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
                  return View(dto);
            }

            // Correction ici : isPersistent est le bon nom de paramètre
            var result = await _signInManager.PasswordSignInAsync(
                user, dto.MotDePasse, isPersistent: dto.Sesouvenir, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                  if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                  return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
            return View(dto);
      }

      // GET /Account/Register
      public IActionResult Register()
      {
            return View();
      }

      // POST /Account/Register
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Register(RegisterDto dto)
      {
            if (!ModelState.IsValid) return View(dto);

            var existant = await _userManager.FindByEmailAsync(dto.Email);
            if (existant != null)
            {
                  ModelState.AddModelError("Email", "Cet email est déjà utilisé.");
                  return View(dto);
            }

            var user = new AppUser
            {
                  UserName = dto.Email,
                  Email = dto.Email,
                  Prenom = dto.Prenom,
                  Nom = dto.Nom,
                  DateInscription = DateTime.Now,
                  Actif = true  // Ajoutez cette ligne si votre modèle l'exige
            };

            var result = await _userManager.CreateAsync(user, dto.MotDePasse);

            if (result.Succeeded)
            {
                  await _userManager.AddToRoleAsync(user, "Employe");
                  await _signInManager.SignInAsync(user, isPersistent: false);
                  TempData["Succes"] = "Compte créé avec succès. Bienvenue !";
                  return RedirectToAction("Index", "Home");
            }

            foreach (var erreur in result.Errors)
                  ModelState.AddModelError(string.Empty, erreur.Description);

            return View(dto);
      }

      // POST /Account/Logout
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Logout()
      {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
      }
}