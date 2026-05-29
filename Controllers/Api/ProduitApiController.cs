using Microsoft.AspNetCore.Mvc;
using GestionStock.Interfaces;
using GestionStock.Models;
using GestionStock.ViewModels;

namespace GestionStock.Controllers.Api;

[ApiController]                          // Active la validation automatique + réponses JSON
[Route("api/[controller]")]              // Route : /api/produit
public class ProduitApiController : ControllerBase
{
      private readonly IProduitService _produitService;

      public ProduitApiController(IProduitService produitService)
      {
            _produitService = produitService;
      }

      // GET /api/produit
      // GET /api/produit?recherche=dell&categorieId=1&stockFaible=true
      [HttpGet]
      [ProducesResponseType(StatusCodes.Status200OK)]
      public async Task<ActionResult<IEnumerable<ProduitDto>>> GetAll(
          string? recherche, int? categorieId, bool? stockFaible)
      {
            var produits = await _produitService.GetAllAsync(recherche, categorieId, stockFaible);

            // Mapping Produit → ProduitDto
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

      // GET /api/produit/5
      [HttpGet("{id}")]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status404NotFound)]
      public async Task<ActionResult<ProduitDto>> GetById(int id)
      {
            var produit = await _produitService.GetByIdAsync(id);
            if (produit == null) return NotFound(new { message = $"Produit {id} introuvable." });

            var dto = new ProduitDto
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
            };

            return Ok(dto);
      }

      // POST /api/produit
      [HttpPost]
      [ProducesResponseType(StatusCodes.Status201Created)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<ActionResult<ProduitDto>> Create([FromBody] ProduitCreateDto dto)
      {
            // [ApiController] valide automatiquement ModelState
            // Si invalide, retourne 400 automatiquement

            if (await _produitService.ReferenceExisteAsync(dto.Reference))
                  return BadRequest(new { message = "Cette référence existe déjà." });

            // Mapping ProduitCreateDto → Produit
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

            // 201 Created avec l'URL du nouveau ressource
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

      // PUT /api/produit/5
      [HttpPut("{id}")]
      [ProducesResponseType(StatusCodes.Status204NoContent)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      [ProducesResponseType(StatusCodes.Status404NotFound)]
      public async Task<IActionResult> Update(int id, [FromBody] ProduitCreateDto dto)
      {
            var produit = await _produitService.GetByIdAsync(id);
            if (produit == null) return NotFound(new { message = $"Produit {id} introuvable." });

            if (await _produitService.ReferenceExisteAsync(dto.Reference, id))
                  return BadRequest(new { message = "Cette référence est déjà utilisée." });

            // Mise à jour des champs
            produit.Reference = dto.Reference;
            produit.Nom = dto.Nom;
            produit.Description = dto.Description;
            produit.PrixUnitaire = dto.PrixUnitaire;
            produit.QuantiteEnStock = dto.QuantiteEnStock;
            produit.SeuilAlerte = dto.SeuilAlerte;
            produit.Actif = dto.Actif;
            produit.CategorieId = dto.CategorieId;

            await _produitService.UpdateAsync(produit);

            return NoContent(); // 204 : succès sans contenu à retourner
      }

      // DELETE /api/produit/5
      [HttpDelete("{id}")]
      [ProducesResponseType(StatusCodes.Status204NoContent)]
      [ProducesResponseType(StatusCodes.Status404NotFound)]
      public async Task<IActionResult> Delete(int id)
      {
            var produit = await _produitService.GetByIdAsync(id);
            if (produit == null) return NotFound(new { message = $"Produit {id} introuvable." });

            await _produitService.DeleteAsync(id);
            return NoContent();
      }
}