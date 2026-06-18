using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ZaryaSite.Extentions;

internal static class UserEndpointExtensions
{
    internal static void ConfigureUserEndpoints(this WebApplication app)
    {
        app.MapPost("/User/Login/Send", async (HttpContext context) =>
        {
            var form = await context.Request.ReadFormAsync();
            var login = form["Login"].ToString();
            var password = form["Password"].ToString();

            if (CheckRegisterAndLoginParam(login, password, out var savedUser, false) is IResult result)
            {
                return result;
            }

            if (savedUser is null)
            {
                return Results.Redirect($"/main?error=wrongLogin&login={Uri.EscapeDataString(login)}");
            }

            await AuthenticationUser(context, savedUser);
            return Results.Redirect("/main");
        });

        app.MapPost("/User/Logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/main");
        });

        app.MapPost("/User/Register/Send", async (HttpContext context) =>
        {
            var form = await context.Request.ReadFormAsync();
            var login = form["Login"].ToString();
            var password = form["Password"].ToString();
            var email = form["Email"].ToString();

            var newUser = new UserInfo()
            {
                Login = login,
                Password = password,
                Email = email,
                Role = Roles.User,
            };

            if (CheckRegisterAndLoginParam(login, password, out var savedUser, true, email) is IResult result)
            {
                return result;
            }

            Program.Saver[login] = newUser;
            await AuthenticationUser(context, newUser);
            return Results.Redirect("/main");
        });
    }

    private static IResult? CheckRegisterAndLoginParam(string login, string password, out UserInfo? user, bool isRegisting, string? email = null)
    {
        user = null;
        var isRegistingValue = isRegisting.ToString().ToLowerInvariant();
        var loginValue = Uri.EscapeDataString(login);
        var emailQuery = isRegisting && !string.IsNullOrEmpty(email)
            ? $"&email={Uri.EscapeDataString(email)}"
            : string.Empty;

        if (string.IsNullOrEmpty(login))
        {
            return Results.Redirect($"/main?error=nullLogin&isRegisting={isRegistingValue}{emailQuery}");
        }

        if (string.IsNullOrEmpty(password))
        {
            return Results.Redirect($"/main?error=nullPass&login={loginValue}&isRegisting={isRegistingValue}{emailQuery}");
        }

        if (!isRegisting && !Program.Saver.TryGetValue(login, out user))
        {
            return Results.Redirect($"/main?error=wrongLogin&login={loginValue}&isRegisting={isRegistingValue}");
        }
        
        if (isRegisting)
        {
            if (Program.Saver.TryGetValue(login, out user))
            {
                return Results.Redirect($"/main?error=wrongLogin&login={loginValue}&isRegisting={isRegistingValue}{emailQuery}");
            }

            if (!string.IsNullOrEmpty(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return Results.Redirect($"/main?error=wrongEmail&login={loginValue}&isRegisting={isRegistingValue}{emailQuery}");
            }
            return null;
        }

        if (user is null || user.Password != password)
        {
            return Results.Redirect($"/main?error=wrongPass&login={loginValue}&isRegisting={isRegistingValue}");
        }

        return null;
    }

    private static async Task AuthenticationUser(HttpContext context, UserInfo user)
    {
        var claims = new Claim[]
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
    }
}
