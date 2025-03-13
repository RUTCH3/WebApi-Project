using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;


namespace WebApi.Managers
{
    public static class TokenService
    {
        private static readonly SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes("SXkSqsKyNUyvGbnHs7ke2NCq8zQzNLW7mPmHbnZZ"));
        public static SymmetricSecurityKey GetKey() { return key; }
        private static readonly string issuer = "https://localhost:5001";
        public static SecurityToken GetToken(List<Claim> claims) =>
            new JwtSecurityToken(
                issuer,
                issuer,
                claims,
                expires: DateTime.Now.AddDays(30.0),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

        public static TokenValidationParameters GetTokenValidationParameters() =>
            new()
            {
                ValidIssuer = issuer,
                ValidAudience = issuer,
                ValidIssuers = ["https://accounts.google.com"], // ✅ זה ה-issuer של גוגל
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                // remove delay of token when expire
                // IssuerSigningKeyResolver = async (token, securityToken, kid, parameters) =>
                // {
                //     using var httpClient = new HttpClient();
                //     var json = await httpClient.GetStringAsync("https://www.googleapis.com/oauth2/v3/certs");
                //     var keys = JsonConvert.DeserializeObject<JsonWebKeySet>(json);
                //     return keys.Keys;
                // },
                IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                {
                    // מבצע קאשינג של המפתחות כדי לא לשאול את גוגל כל פעם מחדש
                    var json = new WebClient().DownloadString("https://www.googleapis.com/oauth2/v3/certs");
                    var keys = JsonConvert.DeserializeObject<JsonWebKeySet>(json);
                    return keys.Keys;
                }
            };

        public static string WriteToken(SecurityToken token) =>
            new JwtSecurityTokenHandler().WriteToken(token);
    }
}