using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GestionStock.Interfaces;
using GestionStock.Models;

namespace GestionStock.Controllers;

public class ProduitController : Controller
{
      private readonly IProduitService _produitService;
      private readonly ICategorieService _categorieService;

      public ProduitController(IProduitService produitService, ICategorieService categorieService)
      {
            _produitService = produitService;
            _categorieService = categorieService;
      }

      // GET /Produit — accessible à tous
      public async Task<IActionResult> Index(string? recherche, int? categorieId, bool? stockFaible)
      {
            var produits = await _produitService.GetAllAsync(recherche, categorieId, stockFaible);
            ViewBag.Recherche = recherche;
            ViewBag.CategorieId = categorieId;
            ViewBag.StockFaible = stockFaible;
            await ChargerCategories(categorieId);
            return View(produits);
      }

      // GET /Produit/Details/5 — accessible à tous
      public async Task<IActionResult> Details(int? id)
      {
            if (id == null) return NotFound();
            var produit = await _produitService.GetByIdAsync(id.Value);
            if (produit == null) return NotFound();
            return View(produit);
      }

      // GET /Produit/Create — connectés seulement
      [Authorize]
      public async Task<IActionResult> Create()
      {
            await ChargerCategories();
            return View();
      }

      // POST /Produit/Create — connectés seulement
      [Authorize]
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Create(
          [Bind("Reference,Nom,Description,PrixUnitaire,QuantiteEnStock,SeuilAlerte,CategorieId,Actif")]
        Produit produit)
      {
            if (await _produitService.ReferenceExisteAsync(produit.Reference))
                  ModelState.AddModelError("Reference", "Cette référence existe déjà.");

            if (ModelState.IsValid)
            {
                  await _produitService.CreateAsync(produit);
                  TempData["Succes"] = $"Produit \"{produit.Nom}\" créé avec succès.";
                  return RedirectToAction(nameof(Index));
            }

            await ChargerCategories(produit.CategorieId);
            return View(produit);
      }

      // GET /Produit/Edit/5 — Admin seulement
      [Authorize(Roles = "Admin")]
      public async Task<IActionResult> Edit(int? id)
      {
            if (id == null) return NotFound();
            var produit = await _produitService.GetByIdAsync(id.Value);
            if (produit == null) return NotFound();
            await ChargerCategories(produit.CategorieId);
            return View(produit);
      }

      // POST /Produit/Edit/5 — Admin seulement
      [Authorize(Roles = "Admin")]
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Edit(int id,
          [Bind("Id,Reference,Nom,Description,PrixUnitaire,QuantiteEnStock,SeuilAlerte,CategorieId,Actif,DateAjout")]
        Produit produit)
      {
            if (id != produit.Id) return NotFound();

            if (await _produitService.ReferenceExisteAsync(produit.Reference, id))
                  ModelState.AddModelError("Reference", "Cette référence est déjà utilisée.");

            if (ModelState.IsValid)
            {
                  await _produitService.UpdateAsync(produit);
                  TempData["Succes"] = $"Produit \"{produit.Nom}\" modifié avec succès.";
                  return RedirectToAction(nameof(Index));
            }

            await ChargerCategories(produit.CategorieId);
            return View(produit);
      }

      // GET /Produit/Delete/5 — Admin seulement
      [Authorize(Roles = "Admin")]
      public async Task<IActionResult> Delete(int? id)
      {
            if (id == null) return NotFound();
            var produit = await _produitService.GetByIdAsync(id.Value);
            if (produit == null) return NotFound();
            return View(produit);
      }

      // POST /Produit/Delete/5 — Admin seulement
      [Authorize(Roles = "Admin")]
      [HttpPost, ActionName("Delete")]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> DeleteConfirmed(int id)
      {
            await _produitService.DeleteAsync(id);
            TempData["Succes"] = "Produit supprimé avec succès.";
            return RedirectToAction(nameof(Index));
      }

      private async Task ChargerCategories(int? selectedId = null)
      {
            var categories = await _categorieService.GetAllAsync();
            ViewBag.CategorieId = new SelectList(categories, "Id", "Nom", selectedId);
      }
}