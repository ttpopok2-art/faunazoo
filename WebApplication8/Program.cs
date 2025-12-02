using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication8.Data;

var builder = WebApplication.CreateBuilder(args);

// --- Контекст Identity ---
builder.Services.AddDbContext<IdentityAppContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("IdentityConnection"),
        new MySqlServerVersion(new Version(8, 0, 34))
    )
);

// --- Контекст твоего зоопарка ---
builder.Services.AddDbContext<FaunaContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("FaunaConnection"),
        new MySqlServerVersion(new Version(8, 0, 34))
    )
);

// Подключаем Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityAppContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// --- Создание ролей и администратора ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    // --- Создаём роли, если их нет ---
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    if (!await roleManager.RoleExistsAsync("User"))
        await roleManager.CreateAsync(new IdentityRole("User"));

    // --- Создаём администратора, если его нет ---
    var adminEmail = "admin@gmail.com";
    var admin = await userManager.FindByEmailAsync(adminEmail);

    if (admin == null)
    {
        admin = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(admin, "Admin123!");
        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
                Console.WriteLine(error.Description);
        }
    }

    // --- Добавляем администратора в роль Admin ---
    if (!await userManager.IsInRoleAsync(admin, "Admin"))
    {
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapRazorPages();

app.Run();
