using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApi.Managers;

namespace WebApi.Controllers;


[Route("google")]
[ApiController]
public class GoogleController : ControllerBase
{
    /// <summary>
    /// התחברות דרך Google
    /// </summary>
    [HttpGet("login")]
    public IActionResult Login()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = "/html/Home.html"//Url.Action(nameof(GoogleResponse)) // חוסך חזרתיות
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// חזרה מגוגל לאחר אימות
    /// </summary>
    // [HttpGet("google-response")]
    // public async Task<IActionResult> GoogleResponse()
    // {
    //     var authResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    //     if (!authResult.Succeeded)
    //     {
    //         return Unauthorized();
    //     }
    //     return Ok(GetUserClaims());
    // }

    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded)
        {
            return RedirectToAction("Login");
        }

        // הדפסת כל הנתונים שמוחזרים כדי לוודא שהם מגיעים כמו שצריך
        var claims = authenticateResult.Principal?.Claims;
        foreach (var claim in claims)
        {
            Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
        }

        var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var token = TokenService.GetToken((List<Claim>)claims);

        return new JsonResult(new { token });
    }

    /// <summary>
    /// החזרת פרטי המשתמש המחובר
    /// </summary>
    [Authorize]
    [HttpGet("userinfo")]
    public IActionResult GetUserInfo()
    {
        return Ok(GetUserClaims());
    }

    /// <summary>
    /// התנתקות מהמערכת
    /// </summary>
    // [Authorize]
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        // await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        // return Ok(new { message = "User logged out successfully" });
        return SignOut(CookieAuthenticationDefaults.AuthenticationScheme);

    }

    /// <summary>
    /// פונקציה עוזרת לשליפת פרטי המשתמש
    /// </summary>
    private object GetUserClaims()
    {
        return new
        {
            name = User.Identity?.Name,
            email = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value
        };
    }
}
