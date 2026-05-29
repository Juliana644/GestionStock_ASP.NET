using Microsoft.EntityFrameworkCore;
using GestionStock.Data;
using GestionStock.Interfaces;
using GestionStock.Services;

var builder = WebApplication.CreateBuilder(args);

// ── SERVICES ──────────────────────────────────────────
builder.Services.AddControllersWithViews();

// EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Enregistrement des services métier
// AddScoped = une instance par requête HTTP
// Le conteneur DI injecte IProduitService partout où c'est demandé
builder.Services.AddScoped<IProduitService, ProduitService>();
builder.Services.AddScoped<ICategorieService, CategorieService>();

var app = builder.Build();

// ── PIPELINE ──────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();