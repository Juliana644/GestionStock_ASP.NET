using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionStock.Data;
using GestionStock.Models;

namespace GestionStock.Controllers;

public class CategorieController : Controller
{
      private readonly ApplicationDbContext _context;
      private readonly ILogger<CategorieController> _logger;

      public CategorieController(ApplicationDbContext context, ILogger<CategorieController> logger)
      {
            _context = context;
            _logger = logger;
      }

      // GET /Categorie
      public async Task<IActionResult> Index()
      {
            // On charge les catégories avec le nombre de produits associés
            var categories = await _context.Categories
                .Include(c => c.Produits)
                .OrderBy(c => c.Nom)
                .ToListAsync();

            return View(categories);
      }

      // GET /Categorie/Details/5
      public async Task<IActionResult> Details(int? id)
      {
            if (id == null) return NotFound();

            var categorie = await _context.Categories
                .Include(c => c.Produits)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categorie == null) return NotFound();
            return View(categorie);
      }

      // GET /Categorie/Create
      public IActionResult Create()
      {
            return View();
      }

      // POST /Categorie/Create
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Create(
          [Bind("Nom,Description")] Categorie categorie)
      {
            // Validation métier : nom unique
            if (await _context.Categories.AnyAsync(c => c.Nom == categorie.Nom))
                  ModelState.AddModelError("Nom", "Une catégorie avec ce nom existe déjà.");

            if (ModelState.IsValid)
            {
                  _context.Add(categorie);
                  await _context.SaveChangesAsync();
                  _logger.LogInformation("Catégorie créée : {Nom}", categorie.Nom);
                  TempData["Succes"] = $"Catégorie \"{categorie.Nom}\" créée avec succès.";
                  return RedirectToAction(nameof(Index));
            }

            return View(categorie);
      }

      // GET /Categorie/Edit/5
      public async Task<IActionResult> Edit(int? id)
      {
            if (id == null) return NotFound();
            var categorie = await _context.Categories.FindAsync(id);
            if (categorie == null) return NotFound();
            return View(categorie);
      }

      // POST /Categorie/Edit/5
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Edit(int id,
          [Bind("Id,Nom,Description")] Categorie categorie)
      {
            if (id != categorie.Id) return NotFound();

            // Validation métier : nom unique (exclure la catégorie actuelle)
            if (await _context.Categories.AnyAsync(c => c.Nom == categorie.Nom && c.Id != id))
                  ModelState.AddModelError("Nom", "Ce nom est déjà utilisé par une autre catégorie.");

            if (ModelState.IsValid)
            {
                  try
                  {
                        _context.Update(categorie);
                        await _context.SaveChangesAsync();
                        TempData["Succes"] = $"Catégorie \"{categorie.Nom}\" modifiée avec succès.";
                  }
                  catch (DbUpdateConcurrencyException)
                  {
                        if (!await _context.Categories.AnyAsync(c => c.Id == categorie.Id))
                              return NotFound();
                        throw;
                  }
                  return RedirectToAction(nameof(Index));
            }

            return View(categorie);
      }

      // GET /Categorie/Delete/5
      public async Task<IActionResult> Delete(int? id)
      {
            if (id == null) return NotFound();

            var categorie = await _context.Categories
                .Include(c => c.Produits)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categorie == null) return NotFound();
            return View(categorie);
      }

      // POST /Categorie/Delete/5
      [HttpPost, ActionName("Delete")]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> DeleteConfirmed(int id)
      {
            var categorie = await _context.Categories
                .Include(c => c.Produits)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categorie != null)
            {
                  // On empêche la suppression si la catégorie a des produits
                  if (categorie.Produits.Any())
                  {
                        TempData["Erreur"] = "Impossible de supprimer une catégorie qui contient des produits.";
                        return RedirectToAction(nameof(Index));
                  }

                  _context.Categories.Remove(categorie);
                  await _context.SaveChangesAsync();
                  TempData["Succes"] = "Catégorie supprimée avec succès.";
            }

            return RedirectToAction(nameof(Index));
      }
}