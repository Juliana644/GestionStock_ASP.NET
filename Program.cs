using Microsoft.EntityFrameworkCore;
using GestionStock.Data;
using GestionStock.Interfaces;
using GestionStock.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ── SERVICES ──────────────────────────────────────────
builder.Services.AddControllersWithViews();

// EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services métier
builder.Services.AddScoped<IProduitService, ProduitService>();
builder.Services.AddScoped<ICategorieService, CategorieService>();

// OpenAPI / Scalar
builder.Services.AddOpenApi();

var app = builder.Build();

// ── PIPELINE ──────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                  // /openapi/v1.json
    app.MapScalarApiReference();       // /scalar/v1
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