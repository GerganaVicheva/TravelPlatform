using Microsoft.AspNetCore.Mvc;
using TravelPlatform.Services.Core.Contracts;
using TravelPlatform.ViewModels;

namespace TravelPlatform.Web.Controllers
{
	public class PostController : BaseController
	{
		private readonly ITravelService _travelService;

		public PostController(ITravelService travelService)
		{
			_travelService = travelService;
		}

		[HttpGet]
		public async Task<IActionResult> Add(string userId)
		{
			try
			{
				var model = new AddPostInputModel();

				return View(model);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);

				return RedirectToAction("Index", "UserAccount", new { userId = GetUserId() });
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Add(AddPostInputModel model)
		{
			try
			{
				string userId = GetUserId();

				if (!ModelState.IsValid)
				{
					return RedirectToAction(nameof(Add), new { userId });
				}

				bool result = await _travelService.AddPostAsync(model, userId);

				if (!result)
				{
					return RedirectToAction(nameof(Add), new { userId });
				}

				return RedirectToAction("Index", "UserAccount", new { userId });
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);

				return RedirectToAction(nameof(Add), new { userId = GetUserId() });
			}
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int postId)
		{
			try
			{
				var model = await _travelService.GetPostEditInfoAsync(postId);

				if (model == null)
				{
					return RedirectToAction("Index", "UserAccount", new { userId = GetUserId() });
				}

				return View(model);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);

				return RedirectToAction("Index", "UserAccount", new { userId = GetUserId() });
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(EditPostInputModel editedModel)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return RedirectToAction(nameof(Edit), new { postId = editedModel.PostId });
				}

				bool result = await _travelService.EditPostAsync(editedModel);

				if (!result)
				{
					return RedirectToAction(nameof(Edit), new { postId = editedModel.PostId });
				}

				return RedirectToAction("Index", "UserAccount", new { userId = GetUserId() });
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);

				return RedirectToAction(nameof(Edit), new { postId = editedModel.PostId });
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int postId)
		{
			try
			{
				bool result = await _travelService.DeletePostAsync(postId);

				return RedirectToAction("Index", "UserAccount", new { userId = GetUserId() });
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);

				return RedirectToAction("Index", "UserAccount", new { userId = GetUserId() });
			}
		}

		[HttpPost]
		public async Task<IActionResult> Like(int postId)
		{
			try
			{
				string userId = GetUserId();

				bool result = await _travelService.LikePostAsync(postId, userId);

				//return RedirectToAction("Index", "UserAccount", new { userId });

				//var postInfo = await _travelService.GetUserProfileInfoAsync(userId);
				//int likesCount = postInfo.Posts.FirstOrDefault(p => p.Id == postId).Likes;

				var likesCount = await _travelService.GetPostLikesCountAsync(postId);

				if (likesCount == null)
				{
					return Json(new { success = false, message = "Post not found" });
				}

				return Json(new { success = true, liked = result, likes = likesCount });
			}
			catch (Exception e)
			{
				//Console.WriteLine(e.Message);
				//return RedirectToAction("Index", "UserAccount", new { userId = GetUserId() });

				return Json(new { success = false, error = e.Message });
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetComments(int postId)
		{
			var comments = await _travelService.GetPostCommentsAsync(postId);

			return PartialView("_CommentListPartial", comments);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddComment(int postId, string content)
		{
			string userId = GetUserId();

			bool added = await _travelService.AddCommentAsync(postId, userId, content);

			if (!added)
			{
				return Json(new { success = false });
			}

			var newComment = await _travelService.GetLastCommentAsync(postId, userId);

			return Json(new { success = true, comment = newComment });
		}
	}
}
