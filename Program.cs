using CMS.Data;
using CMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//EntityFrameworkCore for Postgresql
builder.Services.AddDbContext<ApplicationDbContext>(options =>
     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options=>
    {
    // Customize the password policy here
    options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false; // Disable digit requirement
        options.Password.RequireUppercase = false; // Disable uppercase letter requirement
      options.Password.RequiredLength = 2; // Set minimum password length to 0 to allow any short password

    })
    .AddEntityFrameworkStores<ApplicationDbContext>();
// Configure the application cookie options
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Ensure the LoginPath is set to your login page path
});

var app = builder.Build();

// Seed the admin user
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    SeedAdminUser(userManager).Wait();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

// Add the SeedAdminUser method here (outside the Main method)
 static async Task SeedAdminUser(UserManager<ApplicationUser> userManager)
{
    // Check if the admin user already exists
    var adminUser = await userManager.FindByEmailAsync("admin@gmail.com");
    if (adminUser == null)
    {
        // Create the admin user if it doesn't exist
        var newAdminUser = new ApplicationUser
        {
            UserName = "admin@gmail.com",
            Email = "admin@gmail.com"
            // Add more properties as needed
        };

        // Set the admin password
        string adminPassword = "Admin@123"; // Set the admin's password
        var result = await userManager.CreateAsync(newAdminUser, adminPassword);

        if (result.Succeeded)
        {
            // No roles to assign, as there are no roles in this scenario
        }
    }
}
