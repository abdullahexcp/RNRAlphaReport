using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RNRAlphaReport.DataAccess.ApplicationDbContext;
using RNRAlphaReport.DataAccess.DatabaseUtil;
using RNRAlphaReport.Services;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;


#region Add Services to the container.

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MainConnectionString")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddSingleton<IDatabaseUtil, SQLServerDatabaseUtil>();
builder.Services.AddSingleton<SettingsService>();
builder.Services.AddScoped<ReportBuilderService>();

#endregion

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
