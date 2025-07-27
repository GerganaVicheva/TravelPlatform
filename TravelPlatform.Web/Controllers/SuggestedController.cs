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

		public async Task<IActionResult> Profiles()
        {
			try
			{
				string userId = GetUserId();

				var profiles = await _travelService.GetSuggestedProfilesInfoAsync(userId);

				return View(profiles);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);

				return RedirectToAction("Index", "Home");
			}
        }
    }
}
