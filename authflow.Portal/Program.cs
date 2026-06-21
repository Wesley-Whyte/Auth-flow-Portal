using authflow.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(options =>
{
    // SmartAuth routes to JwtBearer when the custom-API jwt cookie is present,
    // otherwise falls through to EntraCookie so both sign-in paths populate User.
    options.DefaultAuthenticateScheme = "SmartAuth";
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddPolicyScheme("SmartAuth", "SmartAuth", opts =>
{
    opts.ForwardDefaultSelector = ctx =>
        ctx.Request.Cookies.ContainsKey("jwt")
            ? JwtBearerDefaults.AuthenticationScheme
            : "EntraCookie";
})
.AddCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied";
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(
                builder.Configuration["JWT:SigninKey"]
                ?? throw new InvalidOperationException("JWT:SigninKey is not configured")))
    };
    // Read the JWT from the HttpOnly cookie so browser-based requests are authenticated
    // without requiring an Authorization header.
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            ctx.Token = ctx.Request.Cookies["jwt"];
            return Task.CompletedTask;
        }
    };
})
.AddMicrosoftIdentityWebApp(
    builder.Configuration.GetSection("EntraExternalId"),
    openIdConnectScheme: "EntraOidc",
    cookieScheme: "EntraCookie");

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
