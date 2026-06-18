using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ZaryaSite.Extentions;

internal static class UserEndpointExtensions
{
    internal static void ConfigureUserEndpoints(this WebApplication app)
    {
        app.MapPost("/User/Login/Send", async (HttpContext context, ISaver saver) =>
        {
            var form = await context.Request.ReadFormAsync();
            var login = form["Login"].ToString();
            var password = form["Password"].ToString();

            var loginAttempt = new UserInfo()
            {
                Login = login,
                Password = password,
                Role = Roles.User
            };

            if (CheckRegisterAndLoginParam(loginAttempt, false, saver) is IResult result)
            {
                return result;
            }

            var user = saver.Saver[login];
            await AuthenticationUser(context, user);
            return Results.Redirect("/main");
        });

        app.MapPost("/User/Logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/main");
        });

        app.MapPost("/User/Register/Send", async (HttpContext context, ISaver saver) =>
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

            if (CheckRegisterAndLoginParam(newUser, true, saver) is IResult result)
            {
                return result;
            }

            saver.Saver[login] = newUser;
            await AuthenticationUser(context, newUser);
            return Results.Redirect("/main");
        });
    }

    private static IResult? CheckRegisterAndLoginParam(UserInfo user, bool isRegisting, ISaver saver)
    {
        var isRegistingValue = isRegisting.ToString().ToLowerInvariant();
        var loginValue = Uri.EscapeDataString(user.Login);
        var emailQuery = isRegisting && !string.IsNullOrEmpty(user.Email)
            ? $"&email={Uri.EscapeDataString(user.Email)}"
            : string.Empty;

        if (string.IsNullOrEmpty(user.Login))
        {
            return Results.Redirect($"/main?error=nullLogin&isRegisting={isRegistingValue}{emailQuery}");
        }

        if (string.IsNullOrEmpty(user.Password))
        {
            return Results.Redirect($"/main?error=nullPass&login={loginValue}&isRegisting={isRegistingValue}{emailQuery}");
        }

        if (!isRegisting && !saver.Saver.TryGetValue(user.Login, out _))
        {
            return Results.Redirect($"/main?error=wrongLogin&login={loginValue}&isRegisting={isRegistingValue}");
        }
        
        if (isRegisting)
        {
            if (saver.Saver.TryGetValue(user.Login, out _))
            {
                return Results.Redirect($"/main?error=wrongLogin&login={loginValue}&isRegisting={isRegistingValue}{emailQuery}");
            }

            if (!string.IsNullOrEmpty(user.Email) && !Regex.IsMatch(user.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return Results.Redirect($"/main?error=wrongEmail&login={loginValue}&isRegisting={isRegistingValue}{emailQuery}");
            }
            return null;
        }

        if (saver.Saver[user.Login].Password != user.Password)
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
