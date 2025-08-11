using Microsoft.AspNetCore.Mvc;
using TravelPlatform.Services.Core.Contracts;

namespace TravelPlatform.Web.Controllers
{
	public class SuggestedController : BaseController
	{
		private readonly ITravelService _travelService;

		public SuggestedController(ITravelService travelService)
		{
			_travelService = travelService;
		}

		[HttpGet]
		public async Task<IActionResult> Profiles(string searchTerm)
		{
			try
			{
				string userId = GetUserId();

				var profiles = await _travelService.GetSuggestedProfilesInfoAsync(userId);

				if (!string.IsNullOrWhiteSpace(searchTerm))
				{
					profiles = profiles
						.Where(p => p.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
						.ToList();
				}

				return View(profiles);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);

				return RedirectToAction("Index", "Home");
			}
		}

		[HttpGet]
		public async Task<IActionResult> Posts(string searchTerm)
		{
			try
			{
				string userId = GetUserId();

				var posts = await _travelService.GetSuggestedPostsInfoAsync(userId);

				if (!string.IsNullOrWhiteSpace(searchTerm))
				{
					posts = posts
						.Where(p => p.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
							|| p.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
							|| p.ShortDescription.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
						.ToList();
				}

				return View(posts);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);

				return RedirectToAction("Index", "Home");
			}
		}
	}
}
