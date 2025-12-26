using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Taskr.Data;
using Taskr.Models.User;
using Taskr.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")
    ?? throw new InvalidOperationException("AZURE_SQL_CONNECTIONSTRING connection string not found.");

builder.Services.AddDbContext<KanbanDbContext>(options =>
    options.UseSqlServer(connectionString));

// Set up ASP.Net Identity system
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        // ---- Password policy (feel free to relax for a demo) ----
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;

        // ---- Sign‑in options ----
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<KanbanDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddControllersWithViews();

// Board service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BoardController>();

var app = builder.Build();

// Standard middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// **Important** – authentication & authorisation must be added
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Board}/{action=Index}");
app.MapRazorPages();

app.Run();