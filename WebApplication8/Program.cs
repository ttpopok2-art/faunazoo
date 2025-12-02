using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication8.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<IdentityAppContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("IdentityConnection"),
        new MySqlServerVersion(new Version(8, 0, 34))
    )
);

builder.Services.AddDbContext<FaunaContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("FaunaConnection"),
        new MySqlServerVersion(new Version(8, 0, 34))
    )
);


builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{

    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<IdentityAppContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Маршруты
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
app.MapRazorPages();

app.Run();
