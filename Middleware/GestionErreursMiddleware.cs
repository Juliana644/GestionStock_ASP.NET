using System.Net;
using System.Text.Json;

namespace GestionStock.Middleware;

public class GestionErreursMiddleware
{
      private readonly RequestDelegate _next;
      private readonly ILogger<GestionErreursMiddleware> _logger;

      public GestionErreursMiddleware(RequestDelegate next, ILogger<GestionErreursMiddleware> logger)
      {
            _next = next;
            _logger = logger;
      }

      public async Task InvokeAsync(HttpContext context)
      {
            try
            {
                  // Passe la requête au middleware suivant
                  await _next(context);

                  // Gestion des codes HTTP d'erreur sans exception (403, 404...)
                  if (context.Response.StatusCode == 403)
                  {
                        _logger.LogWarning("Accès refusé : {Path} par {User}",
                            context.Request.Path,
                            context.User.Identity?.Name ?? "anonyme");

                        // Si c'est une requête API → JSON, sinon → vue Razor
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                              context.Response.ContentType = "application/json";
                              await context.Response.WriteAsync(JsonSerializer.Serialize(
                                  new { message = "Accès refusé. Vous n'avez pas les droits nécessaires." }));
                        }
                        else
                        {
                              context.Response.Redirect("/Home/Acces403");
                        }
                  }
                  else if (context.Response.StatusCode == 404)
                  {
                        _logger.LogWarning("Page introuvable : {Path}", context.Request.Path);

                        if (!context.Request.Path.StartsWithSegments("/api"))
                              context.Response.Redirect("/Home/Introuvable");
                  }
            }
            catch (Exception ex)
            {
                  // Log de l'exception complète
                  _logger.LogError(ex, "Exception non gérée sur {Method} {Path}",
                      context.Request.Method,
                      context.Request.Path);

                  context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                  if (context.Request.Path.StartsWithSegments("/api"))
                  {
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                              message = "Une erreur interne est survenue.",
                              details = context.RequestServices
                                .GetRequiredService<IWebHostEnvironment>()
                                .IsDevelopment() ? ex.Message : null
                        }));
                  }
                  else
                  {
                        context.Response.Redirect("/Home/Error");
                  }
            }
      }
}