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
	public static async Task Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		var connectionString = builder.Configuration.GetConnectionString("TravelPlatformDbConnection") ?? throw new InvalidOperationException("Connection string 'TravelPlatformDbConnection' not found.");
		builder.Services.AddDbContext<TravelPlatformDbContext>(options =>
			options.UseSqlServer(connectionString));
		builder.Services.AddDatabaseDeveloperPageExceptionFilter();

		builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
			.AddRoles<IdentityRole>()
			.AddEntityFrameworkStores<TravelPlatformDbContext>();
		builder.Services.AddControllersWithViews();

		builder.Services.AddScoped<ITravelService, TravelService>();

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

		// Create roles and assign admin at startup
		using (var scope = app.Services.CreateScope())
		{
			var services = scope.ServiceProvider;

			// Create roles
			await CreateRolesAsync(services);

			// Assign Admin role to a specific user
			await AssignAdminAsync(services);
		}

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

		app.MapControllerRoute(
			name: "areas",
			pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

		app.MapStaticAssets();
		app.MapControllerRoute(
			name: "default",
			pattern: "{controller=Home}/{action=Index}/{id?}")
			.WithStaticAssets();
		app.MapRazorPages()
		   .WithStaticAssets();

		app.Run();
	}

	private static async Task CreateRolesAsync(IServiceProvider serviceProvider)
	{
		var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

		string[] roleNames = { "Administrator", "User" };

		foreach (var roleName in roleNames)
		{
			var roleExist = await roleManager.RoleExistsAsync(roleName);
			if (!roleExist)
			{
				await roleManager.CreateAsync(new IdentityRole(roleName));
			}
		}
	}

	private static async Task AssignAdminAsync(IServiceProvider serviceProvider)
	{
		var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

		var adminUser = await userManager.FindByEmailAsync("admin@example.com");

		if (adminUser != null && !(await userManager.IsInRoleAsync(adminUser, "Administrator")))
		{
			await userManager.AddToRoleAsync(adminUser, "Administrator");
		}
	}
}
