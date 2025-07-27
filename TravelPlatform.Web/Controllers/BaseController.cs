using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TravelPlatform.Web.Controllers
{
	[Authorize]
    public class BaseController : Controller
    {
		protected bool IsUserAuthenticated()
		{
			return User.Identity?.IsAuthenticated ?? false;
		}

		protected string? GetUserId()
		{
			string? userId = null;

			if (IsUserAuthenticated())
			{
				userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			}

			return userId;
		}
	}
}
