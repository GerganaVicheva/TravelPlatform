using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlatform.Web.Data;

namespace TravelPlatform.Web.Areas.Administration.Controllers
{
	[Area("Administration")]
	[Authorize(Roles = "Administrator")]
	public class DashboardController : Controller
    {
		private readonly TravelPlatformDbContext _context;

		public DashboardController(TravelPlatformDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
        {
			var today = DateTime.UtcNow.Date;
			var startDate = today.AddDays(-6); // last 7 days including today

			var travelPosts = Enumerable.Range(0, 7)
				.Select(offset =>
				{
					var date = startDate.AddDays(offset);
					return _context.Posts
						.Where(p => p.CreatedOn.Date == date)
						.Count();
				})
				.ToArray();

			var days = Enumerable.Range(0, 7)
				.Select(offset => startDate.AddDays(offset).ToString("ddd"))
				.ToArray();

			ViewData["Days"] = days;
			ViewData["TravelPosts"] = travelPosts;

			// For demonstration, here we'll just count users who posted on each day as an active user proxy
			var dailyActiveUsers = Enumerable.Range(0, 7)
				.Select(offset =>
				{
					var date = startDate.AddDays(offset);
					return _context.Posts
						.Where(p => p.CreatedOn.Date == date)
						.Select(p => p.UserId)
						.Distinct()
						.Count();
				})
				.ToArray();

			ViewData["DailyActiveUsers"] = dailyActiveUsers;

			return View();
		}
    }
}
