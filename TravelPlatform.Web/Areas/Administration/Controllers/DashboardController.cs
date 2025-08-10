using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlatform.Services.Core.Contracts;
using TravelPlatform.Web.Data;

namespace TravelPlatform.Web.Areas.Administration.Controllers
{
	[Area("Administration")]
	[Authorize(Roles = "Administrator")]
	public class DashboardController : Controller
	{
		private readonly IDashboardService _dashboardService;

		public DashboardController(IDashboardService dashboardService)
		{
			_dashboardService = dashboardService;
		}

		public async Task<IActionResult> Index()
		{
			try
			{
				var (days, travelPosts, dailyActiveUsers) = await _dashboardService.GetDashboardDataAsync();

				ViewData["Days"] = days;
				ViewData["TravelPosts"] = travelPosts;
				ViewData["DailyActiveUsers"] = dailyActiveUsers;

				var model = await _dashboardService.GetDashboardTotalDataAsync();

				return View(model);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);

				return RedirectToAction(nameof(Index), "Home", new { area = "" });
			}
		}
	}
}
