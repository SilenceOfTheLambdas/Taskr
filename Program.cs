using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Taskr.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<KanbanDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("KanbanDb")));

// 2️⃣ Register Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<KanbanDbContext>();

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
    pattern: "{controller=Board}/{action=Index}/{id?}");

app.Run();