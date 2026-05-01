using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SWMSU.Data;
using SWMSU.Interface;
using SWMSU.Service;

var builder = WebApplication.CreateBuilder(args);

// -------------------- Connection String --------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// -------------------- Add DbContext for Identity --------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// -------------------- Developer Exception Page --------------------
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// -------------------- Add Identity --------------------
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // You can set true if you want email confirmation
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// -------------------- Add MVC --------------------
builder.Services.AddControllersWithViews();

// -------------------- Add Session --------------------
builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddScoped<IMail, MailService>();
// -------------------- Register DapperContext --------------------
builder.Services.AddSingleton<DapperContext>();

// -------------------- Build App --------------------
var app = builder.Build();

// -------------------- Configure HTTP Request Pipeline --------------------
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// -------------------- Enable Session --------------------
app.UseSession();

// -------------------- Authentication & Authorization --------------------
app.UseAuthentication();
app.UseAuthorization();

// -------------------- Area Routes --------------------
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// -------------------- Default Route --------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// -------------------- Razor Pages (Identity: Login/Register) --------------------
app.MapRazorPages();

app.Run();
