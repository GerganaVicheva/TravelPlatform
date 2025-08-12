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
		public async Task<IActionResult> Posts(string searchTerm, string country, string town)
		{
			try
			{
				string userId = GetUserId();

				var posts = await _travelService.GetSuggestedPostsInfoAsync(userId);

				var allDestinations = posts.Select(p => p.Destination).DistinctBy(d => d.Country).ToList();
				ViewBag.Countries = allDestinations.Select(d => d.Country).Distinct().OrderBy(c => c).ToList();

				// If a country is chosen, extract towns only from that country
				if (!string.IsNullOrEmpty(country))
				{
					ViewBag.Towns = posts
						.Select(p => p.Destination)
						.Where(d => d.Country.Equals(country, StringComparison.OrdinalIgnoreCase))
						.Select(d => d.Town)
						.Distinct()
						.OrderBy(t => t)
						.ToList();
				}

				// Filtering by country
				if (!string.IsNullOrEmpty(country))
				{
					posts = posts.Where(p => p.Destination.Country.Equals(
						country, StringComparison.OrdinalIgnoreCase));
				}

				// Filtering by town
				if (!string.IsNullOrEmpty(town))
				{
					posts = posts.Where(p => p.Destination.Town.Equals(
						town, StringComparison.OrdinalIgnoreCase));
				}

				// Searching by keywords
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
