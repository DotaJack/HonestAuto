using HonestAuto.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HonestAuto.Models;
using HonestAuto.Hubs;
using HonestAuto.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MarketplaceContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MarketplaceContext")));

// Configure Identity services
builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Add roles to the identity configuration
    .AddEntityFrameworkStores<MarketplaceContext>();
builder.Services.AddScoped<ChatMessageService>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddRouting();
builder.Services.AddMvc();
builder.Services.AddSignalR();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapHub<ChatHub>("/chatHub");
app.UseRouting();
app.UseAuthentication(); // Enable authentication
app.UseAuthorization(); // Enable authorization
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
// Seed Roles and Users
SeedRoles(app.Services).Wait(); // Seed roles
SeedUsersAndRoles(app.Services).Wait(); // Seed users and assign roles

app.Run();

async Task SeedRoles(IServiceProvider serviceProvider)
{
    // Seed roles if they don't exist
    using var scope = serviceProvider.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roleNames = { "User", "Admin" };
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

async Task SeedUsersAndRoles(IServiceProvider serviceProvider)
{
    // Seed users and assign roles
    using var scope = serviceProvider.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Seed roles
    string[] roleNames = { "User", "Admin" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Seed users and assign roles for you to have one admin and one user account pregenerated
    await SeedUserAsync(userManager, "Admin@honestauto.com", "Admin123456!", "Admin");
    await SeedUserAsync(userManager, "User123@gmail.com", "User123465789!", "User");
}

async Task SeedUserAsync(UserManager<User> userManager, string email, string password, string role)
{
    // Seed users and assign roles
    var user = await userManager.FindByEmailAsync(email);
    if (user == null)
    {
        user = new User { UserName = email, Email = email };
        var createUserResult = await userManager.CreateAsync(user, password);
        if (createUserResult.Succeeded)
        {
            var addToRoleResult = await userManager.AddToRoleAsync(user, role);
            if (!addToRoleResult.Succeeded)
            {
                // Handle any errors that occurred during adding the user to the role
                throw new InvalidOperationException($"Error adding user {email} to role {role}");
            }

            // Generate the email confirmation token and confirm the email
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmEmailResult = await userManager.ConfirmEmailAsync(user, code);
            if (!confirmEmailResult.Succeeded)
            {
                throw new InvalidOperationException($"Error confirming email for user {email}");
            }
        }
        else
        {
            // Handle any errors that occurred during user creation
            throw new InvalidOperationException($"Error creating user {email}");
        }
    }
    else
    {
        // User already exists - you might want to check if the email is confirmed or perform other updates
    }
}