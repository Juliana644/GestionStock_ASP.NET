using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionStock.Data;
using GestionStock.Models;

namespace GestionStock.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HomeController> _logger;

    // Le framework injecte automatiquement ApplicationDbContext et ILogger
    public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        // ViewBag : données simples passées à la vue (non typées)
        ViewBag.TotalProduits = await _context.Produits.CountAsync();
        ViewBag.TotalCategories = await _context.Categories.CountAsync();
        ViewBag.TotalMouvements = await _context.MouvementsStock.CountAsync();
        ViewBag.StockFaible = await _context.Produits
            .Where(p => p.QuantiteEnStock <= p.SeuilAlerte && p.Actif)
            .CountAsync();

        // Model : liste typée passée à la vue (produits critiques)
        var produitsCritiques = await _context.Produits
            .Include(p => p.Categorie)        // JOIN avec Categories
            .Where(p => p.QuantiteEnStock <= p.SeuilAlerte && p.Actif)
            .OrderBy(p => p.QuantiteEnStock)
            .Take(5)
            .ToListAsync();

        return View(produitsCritiques);
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