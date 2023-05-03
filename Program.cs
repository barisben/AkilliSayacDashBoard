using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AkilliSayac.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using AkilliSayac.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AkilliSayac.Areas.Identity.Data;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthorization(options => {
    options.AddPolicy("readpolicy",
        builder => builder.RequireRole("Admin", "SuperAdmin"));
    options.AddPolicy("writepolicy",
        builder => builder.RequireRole("SuperAdmin"));
});


builder.Services.AddIdentity<AkilliSayacUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(1000);
})
                 .AddDefaultUI()
                 .AddEntityFrameworkStores<ApplicationDbContext>()
                 .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.Cookie.MaxAge = options.ExpireTimeSpan;
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
});

builder.Services.AddScoped<ChangePasswordResourceFilter>();

builder.Services.AddMvc(o =>
{
    o.Filters.Add(typeof(ChangePasswordResourceFilter));
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetService(typeof(UserManager<AkilliSayacUser>));
    var roleManager = scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>));
    var hostingEnvironment = scope.ServiceProvider.GetService(typeof(IWebHostEnvironment));
    var db = scope.ServiceProvider.GetService(typeof(ApplicationDbContext));

    ContextSeed.Seed((UserManager<AkilliSayacUser>)userManager, (RoleManager<IdentityRole>)roleManager, (ApplicationDbContext)db);
    LogOperation.GetAllLogsFromFiles((ApplicationDbContext)db, (IWebHostEnvironment)hostingEnvironment);
    AnomalyOperation.GetAnomaliesFromFile((ApplicationDbContext)db, (IWebHostEnvironment)hostingEnvironment);
    MalwareOperation.GetMalwaresFromFile((ApplicationDbContext)db, (IWebHostEnvironment)hostingEnvironment);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();