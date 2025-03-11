using WebApi.Managers;
using WebApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// הגדרת HTTPS
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5000); // HTTP
    options.ListenLocalhost(5001, listenOptions => { listenOptions.UseHttps(); });
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Jewelry", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter JWT with Bearer into field",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer"}
                },
                Array.Empty<string>()
            }
        });
        // הגדרת OAuth2 בגוגל
        c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/auth"),
                    TokenUrl = new Uri("https://oauth2.googleapis.com/token"),
                    Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID Connect" },
                    { "profile", "Access profile information" },
                    { "email", "Access email address" }
                }
                }
            }
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new List<string>() { "openid", "profile", "email" }
        }

    });
    });

// Add services of Jewelry and users
builder.Services.AddSingleton<IJewelryService, JewelryService>();
builder.Services.AddSingleton<IUserService, UserService>();

// טעינת הגדרות מגוגל ו-JWT
var googleAuthConfig = builder.Configuration.GetSection("Authentication:Google");
var jwtConfig = builder.Configuration.GetSection("Authentication:Jwt");
var clientId = googleAuthConfig["ClientId"];
var clientSecret = googleAuthConfig["ClientSecret"];

// הוספת אימות דרך Google
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // שינוי מ-Cookie ל-JWT
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // השתמש ב-JWT כברירת מחדל
})
.AddJwtBearer(options =>
{
    options.Authority = "https://accounts.google.com";
    options.MetadataAddress = "https://accounts.google.com/.well-known/openid-configuration";
    options.Audience = clientId;
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = TokenService.GetTokenValidationParameters();
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = clientId;
    options.ClientSecret = clientSecret;
    options.CallbackPath = "/signin-google";
    options.BackchannelTimeout = TimeSpan.FromMinutes(5);
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
    options.Scope.Add("https://www.googleapis.com/auth/userinfo.email");

    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
    options.ClaimActions.MapJsonKey("urn:google:picture", "picture");
    options.ClaimActions.MapJsonKey("urn:google:given_name", "given_name");
    options.ClaimActions.MapJsonKey("urn:google:family_name", "family_name");

    options.Events.OnCreatingTicket = context =>
    {
        var identity = (ClaimsIdentity)context.Principal.Identity;
        identity.AddClaim(new Claim(ClaimTypes.Role, "User"));
        return Task.CompletedTask;
    };
});
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
// })
// .AddCookie()
// .AddGoogle(options =>
// {
//     options.ClientId = clientId;
//     options.ClientSecret = clientSecret;
//     options.CallbackPath = "/signin-google";
//     options.BackchannelTimeout = TimeSpan.FromMinutes(2); // הארכת זמן ההמתנה
//     options.Scope.Add("openid");
//     options.Scope.Add("profile"); // גישה לפרופיל
//     options.Scope.Add("email"); // גישה לאימייל

//     // הוספת scope כדי לקבל את התמונה של המשתמש
//     options.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
//     options.Scope.Add("https://www.googleapis.com/auth/userinfo.email");

//     options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
//     options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
//     options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
//     options.ClaimActions.MapJsonKey("urn:google:picture", "picture"); // מיפוי התמונה
//     options.ClaimActions.MapJsonKey("urn:google:given_name", "given_name");
//     options.ClaimActions.MapJsonKey("urn:google:family_name", "family_name");
//     options.Events.OnCreatingTicket = context =>
//     {
//         var identity = (ClaimsIdentity)context.Principal.Identity;
//         identity.AddClaim(new Claim(ClaimTypes.Role, "User")); // הוספת תפקיד "User" לכל משתמש שמתחבר
//         return Task.CompletedTask;
//     };

// })
// .AddJwtBearer(options =>
// {
//     options.Authority = "https://accounts.google.com";
//     options.MetadataAddress = "https://accounts.google.com/.well-known/openid-configuration";
//     options.Audience = clientId;
//     options.RequireHttpsMetadata = true;
//     options.SaveToken = true;
//     options.TokenValidationParameters = TokenService.GetTokenValidationParameters();
// });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", policy => policy.RequireClaim("type", "Admin"))
    .AddPolicy("User", policy => policy.RequireClaim("type", "User"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// builder.Host.UseSerilog((context, config) =>
// {
//     config
//         .WriteTo.Console()
//         .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
//         .MinimumLevel.Debug();
// });

var app = builder.Build();

app.UseCors("AllowAll");
// app.UseErrorMiddleware();
// app.UseLogMiddleware();
// app.UseMiddleware<LogMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");// הגדרת Google OAuth ב-Swagger UI
        c.OAuthClientId(clientId);
        c.OAuthClientSecret(clientSecret);
        c.OAuthAppName("My API - Swagger UI");
        c.OAuthUsePkce();
    });
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
