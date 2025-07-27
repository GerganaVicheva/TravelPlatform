using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TravelPlatform.Web.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using TravelPlatform.Services.Core.Contracts;
using TravelPlatform.Services.Core;
using TravelPlatform.Data.Models;

namespace TravelPlatform.Web;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		var connectionString = builder.Configuration.GetConnectionString("TravelPlatformDbConnection") ?? throw new InvalidOperationException("Connection string 'TravelPlatformDbConnection' not found.");
		builder.Services.AddDbContext<TravelPlatformDbContext>(options =>
			options.UseSqlServer(connectionString));
		builder.Services.AddDatabaseDeveloperPageExceptionFilter();

		builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
			.AddEntityFrameworkStores<TravelPlatformDbContext>();
		builder.Services.AddControllersWithViews();

		builder.Services.AddScoped<ITravelService, TravelService>();

		// Fix: Ensure the required Google authentication package is referenced and the namespace is included.
		builder.Services.AddAuthentication().AddGoogle(googleOptions =>
		{
			googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
			googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
		});

		builder.Services.AddAuthentication().AddFacebook(facebookOptions =>
		{
			facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
			facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
		});

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseMigrationsEndPoint();
		}
		else
		{
			app.UseExceptionHandler("/Home/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapStaticAssets();
		app.MapControllerRoute(
			name: "default",
			pattern: "{controller=Home}/{action=Index}/{id?}")
			.WithStaticAssets();
		app.MapRazorPages()
		   .WithStaticAssets();

		app.Run();
	}
}
