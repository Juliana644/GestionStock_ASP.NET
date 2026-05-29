using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionStock.Data;
using GestionStock.Interfaces;
using GestionStock.Models;

namespace GestionStock.Controllers;

public class HomeController : Controller
{
    private readonly IProduitService _produitService;
    private readonly ICategorieService _categorieService;
    private readonly ApplicationDbContext _context;

    public HomeController(
        IProduitService produitService,
        ICategorieService categorieService,
        ApplicationDbContext context)
    {
        _produitService = produitService;
        _categorieService = categorieService;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalProduits = (await _produitService.GetAllAsync(null, null, null)).Count();
        ViewBag.TotalCategories = (await _categorieService.GetAllAsync()).Count();
        ViewBag.StockFaible = await _produitService.CountStockFaibleAsync();
        ViewBag.TotalMouvements = await _context.MouvementsStock.CountAsync();

        var produitsCritiques = await _produitService.GetAllAsync(null, null, true);
        return View(produitsCritiques.Take(5));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}