using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Taskr.Data;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Add EF Core (choose your provider – SQLite works well for a demo)
builder.Services.AddDbContext<KanbanDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2️⃣ Register Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        // optional: tweak password rules, lockout, etc.
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<KanbanDbContext>()
    .AddDefaultTokenProviders();

// 3️⃣ Add MVC (or Razor Pages) after Identity so it can use the auth services
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Standard middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// **Important** – authentication & authorization must be added
app.UseAuthentication();   // <-- adds UserManager, SignInManager, etc.
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();