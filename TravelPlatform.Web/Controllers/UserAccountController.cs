using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TravelPlatform.Data.Models;
using TravelPlatform.Services.Core.Contracts;
using TravelPlatform.ViewModels;

namespace TravelPlatform.Web.Controllers
{
	public class UserAccountController : BaseController
	{
		private readonly ITravelService _travelService;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public UserAccountController(ITravelService travelService, 
			SignInManager<ApplicationUser> signInManager)
		{
			_travelService = travelService;
			_signInManager = signInManager;
		}

		[HttpGet]
		public async Task<IActionResult> Index(string userId)
		{
			try
			{
				var model = await _travelService.GetUserProfileInfoAsync(userId);

				ViewData["CurrentUserId"] = GetUserId();

				return View(model);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);

				return RedirectToAction("Index", "Home");
			}
		}

		[HttpGet]
		public async Task<IActionResult> Edit(string userId)
		{
			try
			{
				ViewData["CurrentUserId"] = GetUserId();

				var model = await _travelService.GetUserProfileEditInfoAsync(userId);

				return View(model);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);

				return RedirectToAction(nameof(Index), new { userId });
			}
		}

		[HttpPost]
		public async Task<IActionResult> Edit(EditAccountInputModel editedModel)
		{
			try
			{
				string userId = GetUserId();

				if (!ModelState.IsValid)
				{
					return RedirectToAction(nameof(Edit), new { userId });
				}

				var operationResult = await _travelService.EditUserProfileAsync(editedModel, userId);

				if (!operationResult)
				{
					return RedirectToAction(nameof(Edit), new { userId });
				}

				return RedirectToAction(nameof(Index), new { userId });
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);

				return RedirectToAction(nameof(Edit), new { userId = GetUserId() });
			}
		}

		[HttpPost]
		public async Task<IActionResult> Delete(string userId)
		{
			try
			{
				bool result = await _travelService.DeleteProfileAsync(userId);

				if (!result)
				{
					return RedirectToAction(nameof(Index), new { userId });
				}

				await _signInManager.SignOutAsync();
				return RedirectToAction("Index", "Home");
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				
				return RedirectToAction(nameof(Index), new { userId });
			}
		}

	}
}
