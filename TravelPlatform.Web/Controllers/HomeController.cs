using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelPlatform.Services.Core.Contracts;
using TravelPlatform.ViewModels;
using TravelPlatform.Web.Models;

namespace TravelPlatform.Web.Controllers;

public class HomeController : BaseController
{
    private readonly ITravelService _travelService;

    public HomeController(ITravelService travelService)
    {
        _travelService = travelService;
    }

    [HttpGet]
    [AllowAnonymous]
	public IActionResult Index()
    {
        return View();
    }

	[HttpGet]
	[AllowAnonymous]
	public IActionResult Contact()
	{
		return View();
	}

    [HttpPost]
    [AllowAnonymous]
	[ValidateAntiForgeryToken]
    public async Task<IActionResult> SendMessage(AddMessageInputModel model)
    {
        if (!ModelState.IsValid)
        {
			return Json(new { success = false, message = "Please fill in all required fields correctly." });
		}

        bool isSent = await _travelService.SendMessageAsync(model);

        if (isSent)
        {
			return Json(new { success = true, message = "Message sent successfully!" });

		}
        else
        {
			return Json(new { success = false, message = "Something went wrong." });
		}
    }

	[HttpGet]
	[AllowAnonymous]
	public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
