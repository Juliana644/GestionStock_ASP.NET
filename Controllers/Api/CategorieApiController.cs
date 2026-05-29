using Microsoft.AspNetCore.Mvc;
using GestionStock.Interfaces;
using GestionStock.Models;
using GestionStock.ViewModels;

namespace GestionStock.Controllers.Api;

[ApiController]
[Route("api/[controller]")]   // Route : /api/categorie
public class CategorieApiController : ControllerBase
{
      private readonly ICategorieService _categorieService;

      public CategorieApiController(ICategorieService categorieService)
      {
            _categorieService = categorieService;
      }

      // GET /api/categorie
      [HttpGet]
      [ProducesResponseType(StatusCodes.Status200OK)]
      public async Task<ActionResult<IEnumerable<CategorieDto>>> GetAll()
      {
            var categories = await _categorieService.GetAllAsync();

            var dtos = categories.Select(c => new CategorieDto
            {
                  Id = c.Id,
                  Nom = c.Nom,
                  Description = c.Description,
                  NombreProduits = c.Produits.Count
            });

            return Ok(dtos);
      }

      // GET /api/categorie/5
      [HttpGet("{id}")]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status404NotFound)]
      public async Task<ActionResult<CategorieDto>> GetById(int id)
      {
            var categorie = await _categorieService.GetByIdAsync(id);
            if (categorie == null) return NotFound(new { message = $"Catégorie {id} introuvable." });

            return Ok(new CategorieDto
            {
                  Id = categorie.Id,
                  Nom = categorie.Nom,
                  Description = categorie.Description,
                  NombreProduits = categorie.Produits.Count
            });
      }

      // POST /api/categorie
      [HttpPost]
      [ProducesResponseType(StatusCodes.Status201Created)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public async Task<ActionResult<CategorieDto>> Create([FromBody] CategorieCreateDto dto)
      {
            if (await _categorieService.NomExisteAsync(dto.Nom))
                  return BadRequest(new { message = "Une catégorie avec ce nom existe déjà." });

            var categorie = new Categorie
            {
                  Nom = dto.Nom,
                  Description = dto.Description
            };

            await _categorieService.CreateAsync(categorie);

            return CreatedAtAction(nameof(GetById), new { id = categorie.Id }, new CategorieDto
            {
                  Id = categorie.Id,
                  Nom = categorie.Nom,
                  Description = categorie.Description
            });
      }

      // PUT /api/categorie/5
      [HttpPut("{id}")]
      [ProducesResponseType(StatusCodes.Status204NoContent)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      [ProducesResponseType(StatusCodes.Status404NotFound)]
      public async Task<IActionResult> Update(int id, [FromBody] CategorieCreateDto dto)
      {
            var categorie = await _categorieService.GetByIdAsync(id);
            if (categorie == null) return NotFound(new { message = $"Catégorie {id} introuvable." });

            if (await _categorieService.NomExisteAsync(dto.Nom, id))
                  return BadRequest(new { message = "Ce nom est déjà utilisé par une autre catégorie." });

            categorie.Nom = dto.Nom;
            categorie.Description = dto.Description;

            await _categorieService.UpdateAsync(categorie);
            return NoContent();
      }

      // DELETE /api/categorie/5
      [HttpDelete("{id}")]
      [ProducesResponseType(StatusCodes.Status204NoContent)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      [ProducesResponseType(StatusCodes.Status404NotFound)]
      public async Task<IActionResult> Delete(int id)
      {
            var categorie = await _categorieService.GetByIdAsync(id);
            if (categorie == null) return NotFound(new { message = $"Catégorie {id} introuvable." });

            if (await _categorieService.HasProduitsAsync(id))
                  return BadRequest(new { message = "Impossible de supprimer une catégorie qui contient des produits." });

            await _categorieService.DeleteAsync(id);
            return NoContent();
      }
}