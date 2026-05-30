using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestionStock.Interfaces;
using GestionStock.Models;
using GestionStock.ViewModels;

namespace GestionStock.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class ProduitApiController : ControllerBase
{
      private readonly IProduitService _produitService;

      public ProduitApiController(IProduitService produitService)
      {
            _produitService = produitService;
      }

      // GET /api/produit — public
      [HttpGet]
      public async Task<ActionResult<IEnumerable<ProduitDto>>> GetAll(
          string? recherche, int? categorieId, bool? stockFaible)
      {
            var produits = await _produitService.GetAllAsync(recherche, categorieId, stockFaible);
            var dtos = produits.Select(p => new ProduitDto
            {
                  Id = p.Id,
                  Reference = p.Reference,
                  Nom = p.Nom,
                  Description = p.Description,
                  PrixUnitaire = p.PrixUnitaire,
                  QuantiteEnStock = p.QuantiteEnStock,
                  SeuilAlerte = p.SeuilAlerte,
                  Actif = p.Actif,
                  DateAjout = p.DateAjout,
                  CategorieId = p.CategorieId,
                  CategorieNom = p.Categorie?.Nom
            });
            return Ok(dtos);
      }

      // GET /api/produit/5 — public
      [HttpGet("{id}")]
      public async Task<ActionResult<ProduitDto>> GetById(int id)
      {
            var produit = await _produitService.GetByIdAsync(id);
            if (produit == null)
                  return NotFound(new { message = $"Produit {id} introuvable." });

            return Ok(new ProduitDto
            {
                  Id = produit.Id,
                  Reference = produit.Reference,
                  Nom = produit.Nom,
                  Description = produit.Description,
                  PrixUnitaire = produit.PrixUnitaire,
                  QuantiteEnStock = produit.QuantiteEnStock,
                  SeuilAlerte = produit.SeuilAlerte,
                  Actif = produit.Actif,
                  DateAjout = produit.DateAjout,
                  CategorieId = produit.CategorieId,
                  CategorieNom = produit.Categorie?.Nom
            });
      }

      // POST /api/produit — connecté requis
      [Authorize(AuthenticationSchemes = "Bearer")]
      [HttpPost]
      public async Task<ActionResult<ProduitDto>> Create([FromBody] ProduitCreateDto dto)
      {
            if (await _produitService.ReferenceExisteAsync(dto.Reference))
                  return BadRequest(new { message = "Cette référence existe déjà." });

            var produit = new Produit
            {
                  Reference = dto.Reference,
                  Nom = dto.Nom,
                  Description = dto.Description,
                  PrixUnitaire = dto.PrixUnitaire,
                  QuantiteEnStock = dto.QuantiteEnStock,
                  SeuilAlerte = dto.SeuilAlerte,
                  Actif = dto.Actif,
                  CategorieId = dto.CategorieId
            };

            await _produitService.CreateAsync(produit);

            return CreatedAtAction(nameof(GetById), new { id = produit.Id }, new ProduitDto
            {
                  Id = produit.Id,
                  Reference = produit.Reference,
                  Nom = produit.Nom,
                  Description = produit.Description,
                  PrixUnitaire = produit.PrixUnitaire,
                  QuantiteEnStock = produit.QuantiteEnStock,
                  SeuilAlerte = produit.SeuilAlerte,
                  Actif = produit.Actif,
                  DateAjout = produit.DateAjout,
                  CategorieId = produit.CategorieId
            });
      }

      // PUT /api/produit/5 — Admin requis
      [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
      [HttpPut("{id}")]
      public async Task<IActionResult> Update(int id, [FromBody] ProduitCreateDto dto)
      {
            var produit = await _produitService.GetByIdAsync(id);
            if (produit == null)
                  return NotFound(new { message = $"Produit {id} introuvable." });

            if (await _produitService.ReferenceExisteAsync(dto.Reference, id))
                  return BadRequest(new { message = "Cette référence est déjà utilisée." });

            produit.Reference = dto.Reference;
            produit.Nom = dto.Nom;
            produit.Description = dto.Description;
            produit.PrixUnitaire = dto.PrixUnitaire;
            produit.QuantiteEnStock = dto.QuantiteEnStock;
            produit.SeuilAlerte = dto.SeuilAlerte;
            produit.Actif = dto.Actif;
            produit.CategorieId = dto.CategorieId;

            await _produitService.UpdateAsync(produit);
            return NoContent();
      }

      // DELETE /api/produit/5 — Admin requis
      [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
      [HttpDelete("{id}")]
      public async Task<IActionResult> Delete(int id)
      {
            var produit = await _produitService.GetByIdAsync(id);
            if (produit == null)
                  return NotFound(new { message = $"Produit {id} introuvable." });

            await _produitService.DeleteAsync(id);
            return NoContent();
      }
}