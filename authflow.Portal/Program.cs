using authflow.Infrastructure;
using DotNetEnv;
using Microsoft.Identity.Web;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "EntraCookie";
    options.DefaultChallengeScheme = "EntraOidc";
    options.DefaultSignOutScheme = "EntraCookie";
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
