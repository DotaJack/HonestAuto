using HonestAuto.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HonestAuto.Models;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MarketplaceContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MarketplaceContext")));

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<MarketplaceContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Ensure UseAuthentication is called before UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

SeedRoles(app.Services).Wait();

app.Run();

async Task SeedRoles(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roleNames = { "User", "Admin" };
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            // Create the roles and seed them to the database
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}