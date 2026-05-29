using Microsoft.EntityFrameworkCore;
using GestionStock.Data;

var builder = WebApplication.CreateBuilder(args);

// ── ENREGISTREMENT DES SERVICES ──────────────────────
// MVC avec vues Razor
builder.Services.AddControllersWithViews();

// EF Core : on lit la connexion depuis appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// ── PIPELINE DE REQUÊTES HTTP ─────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Route par défaut : /Produit/Index, /Home/Index, etc.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Migration automatique au démarrage (crée la BDD si elle n'existe pas)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();