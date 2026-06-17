using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace ZaryaSite.Extentions;

public static class AdminEndpointExtensions
{
    public static void ConfigureAdminEndpoints(this WebApplication app)
    {
        app.MapGet("/Admin/Login", (IWebHostEnvironment env) =>
        {
            return Results.File(
                Path.Combine(env.WebRootPath, "html", "LoginAndRegister.html"),
                "text/html; charset=utf-8");
        });

        app.MapPost("/Admin/Login/Send", (Delegate)OnPostLoginAdmin);

        app.MapPost("/Admin/Logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/main");
        });
    }

    private static async Task<IResult> OnPostLoginAdmin(HttpContext context)
    {
        var form = await context.Request.ReadFormAsync();
        var login = form["Login"].ToString();
        var pass = form["Password"].ToString();

        if (CheckAdminLoginAndPass(login, pass) is IResult result)
        {
            return result;
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Role, "Admin")
        };

        var claimsIdentity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        return Results.Redirect("/main");
    }

    private static IResult? CheckAdminLoginAndPass(string login, string pass)
    {
        if (string.IsNullOrEmpty(login))
        {
            return Results.Redirect($"/Admin/Login?error=nullLogin&login={login}");
        }
        else if (string.IsNullOrEmpty(pass))
        {
            return Results.Redirect($"/Admin/Login?error=nullPass&login={login}");
        }
        else if (!Program.Saver.ContainsKey(login))
        {
            return Results.Redirect($"/Admin/Login?error=wrongLogin&login={login}");
        }
        else if (!Program.Saver.TryGetValue(login, out var savedPass) || savedPass != pass)
        {
            return Results.Redirect($"/Admin/Login?error=wrongPass&login={login}");
        }
        return null;
    }
}
