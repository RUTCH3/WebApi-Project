using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Controllers;


[Route("[controller]")]
[ApiController]
public class GoogleController : ControllerBase
{
    /// <summary>
    /// התחברות דרך Google
    /// </summary>
    [HttpGet]
    [Route("[action]")]
    public IActionResult Login()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = "/html/Home.html"//Url.Action(nameof(GoogleResponse)) // חוסך חזרתיות
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded)
        {
            var failureMessage = result.Failure?.Message ?? "Unknown error";
            Console.WriteLine($"Authentication failed: {failureMessage}");
            return BadRequest($"Authentication failed: {failureMessage}");
        }

        var claims = result.Principal.Identities
            .FirstOrDefault()?.Claims.Select(c => new { c.Type, c.Value });

        var picture = claims.FirstOrDefault(c => c.Type == "picture")?.Value;

        var pictureUrl = claims.FirstOrDefault(c => c.Type == "urn:google:picture")?.Value;
        Console.WriteLine($"User Profile Picture: {pictureUrl}");
        foreach (var claim in claims)
        {
            Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
        }

        // קבלי את טוקן הגישה של גוגל
        var googleAccessToken = result.Properties.GetTokenValue("access_token");

        // צור טוקן משלך
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("ne3i8d7g28tb_ui34cjmyn7tgy");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
            new Claim(ClaimTypes.NameIdentifier, result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? ""),
            new Claim(ClaimTypes.Name, result.Principal.Identity?.Name ?? ""),
            new Claim("GoogleAccessToken", googleAccessToken ?? ""),
            new Claim("Type","Admin"),
            new Claim("Type","User")

        ]),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return Ok(new { Token = jwtToken, GoogleAccessToken = googleAccessToken, Claims = claims });
    }

    [HttpGet("profile-picture")]
    public async Task<IActionResult> GetProfilePicture()
    {
        var pictureUrl = User.Claims.FirstOrDefault(c => c.Type == "urn:google:picture")?.Value;
        if (string.IsNullOrEmpty(pictureUrl))
        {
            return NotFound(new { message = "Profile picture not found" });
        }

        using HttpClient client = new();
        var imageBytes = await client.GetByteArrayAsync(pictureUrl);
        Console.WriteLine("image bytes: " + imageBytes);
        return File(imageBytes, "image/jpeg"); // או image/png
    }

    /// <summary>
    /// התנתקות מהמערכת
    /// </summary>
    [HttpGet]
    [Route("[action]")]
    public IActionResult Logout()
    {
        // await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        // return Ok(new { message = "User logged out successfully" });
        return SignOut(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
