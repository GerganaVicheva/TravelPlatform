using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
				string currentUserId = GetUserId();

				var model = await _travelService.GetUserProfileInfoAsync(userId, currentUserId);

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
		[ValidateAntiForgeryToken]
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
		[ValidateAntiForgeryToken]
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

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Follow(string followedUserId)
		{
			try
			{
				var currentUserId = GetUserId();

				if (string.IsNullOrEmpty(followedUserId) || followedUserId == currentUserId)
				{
					return Json(new { success = false, message = "Invalid follow request." });
				}

				var isNowFollowing = await _travelService.FollowAsync(currentUserId, followedUserId);

				return Json(new { success = true, isFollowing = isNowFollowing });
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);

				return Json(new { success = false, message = "Unable to process the follow request." });
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetFollowersPartial(string userId)
		{
			try
			{
				string currentUserId = GetUserId();

				var followers = await _travelService.GetFollowersAsync(userId, currentUserId);

				return PartialView("_FollowListPartial", followers);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);

				return BadRequest("Failed to load followers.");
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetFollowingPartial(string userId)
		{
			try
			{
				string currentUserId = GetUserId();

				var following = await _travelService.GetFollowingAsync(userId, currentUserId);

				return PartialView("_FollowListPartial", following);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);

				return BadRequest("Failed to load followers.");
			}
		}

	}
}
