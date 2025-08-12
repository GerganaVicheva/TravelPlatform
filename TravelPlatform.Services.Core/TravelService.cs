using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net.Mail;
using System.Runtime.InteropServices;
using TravelPlatform.Data.Models;
using TravelPlatform.Services.Core.Contracts;
using TravelPlatform.ViewModels;
using TravelPlatform.Web.Data;
using static TravelPlatform.GCommon.ValidationConstants.CommentValidationConstants;

namespace TravelPlatform.Services.Core
{
	public class TravelService : ITravelService
	{
		private readonly TravelPlatformDbContext _dbContext;

		public TravelService(TravelPlatformDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<bool> SendMessageAsync(AddMessageInputModel model)
		{
			bool result = false;

			bool isEmailValid = IsEmailValid(model.Email);
			bool isModelValid = !string.IsNullOrEmpty(model.FirstName)
				&& !string.IsNullOrEmpty(model.LastName)
				&& !string.IsNullOrEmpty(model.Subject)
				&& !string.IsNullOrEmpty(model.Content);

			if (isEmailValid && isModelValid)
			{
				MessageContact message = new MessageContact()
				{
					FirstName = model.FirstName,
					LastName = model.LastName,
					Email = model.Email,
					Subject = model.Subject,
					Content = model.Content,
					SentAt = DateTime.UtcNow
				};

				await _dbContext.MessagesContacts.AddAsync(message);
				await _dbContext.SaveChangesAsync();

				result = true;
			}

			return result;
		}

		private static bool IsEmailValid(string email)
		{
			var valid = true;

			try
			{
				var emailAddress = new MailAddress(email);
			}
			catch
			{
				valid = false;
			}

			return valid;
		}

		public async Task<UserProfileViewModel> GetUserProfileInfoAsync(string userId, string currentUserId)
		{
			var user = await _dbContext.ApplicationUsers
			.Include(u => u.Followers)
			.ThenInclude(f => f.Follower)
			.Include(u => u.Following)
			.ThenInclude(f => f.Following)
			.FirstOrDefaultAsync(u => u.Id == userId);

			var posts = await _dbContext.Posts
				.Where(p => p.UserId == userId)
				.Include(p => p.Destination)
				.Include(p => p.Comments)
				.Include(p => p.Likes)
				.OrderByDescending(p => p.CreatedOn)
				.Select(p => new UserPostViewModel
				{
					Id = p.Id,
					Title = p.Title,
					Content = p.Content,
					ImageUrl = p.ImageUrl,
					CreatedOn = p.CreatedOn,
					Likes = p.Likes.Count,
					Comments = p.Comments.Count,
					DestinationName = p.Destination.Name,
					Town = p.Destination.Town,
					Country = p.Destination.Country,
					//IsLikedByCurrentUser = _dbContext.Likes
					//	.Any(l => l.PostId == p.Id && l.UserId == userId)
					IsLikedByCurrentUser = p.Likes.Any(l => l.UserId == currentUserId)
				})
				.ToListAsync();

			var followers = user.Followers.Select(f => new SuggestedProfilesViewModel
			{
				UserId = f.FollowerId,
				Username = f.Follower.UserName,
				ProfilePictureUrl = f.Follower.ProfilePictureUrl,
				Bio = f.Follower.Bio,
				IsFollowing = _dbContext.Follows.Any(x => x.FollowerId == currentUserId && x.FollowingId == f.FollowerId)
			}).ToList();

			var following = user.Following.Select(f => new SuggestedProfilesViewModel
			{
				UserId = f.FollowingId,
				Username = f.Following.UserName,
				ProfilePictureUrl = f.Following.ProfilePictureUrl,
				Bio = f.Following.Bio,
				IsFollowing = _dbContext.Follows.Any(x => x.FollowerId == currentUserId && x.FollowingId == f.FollowingId)
			}).ToList();

			var profileInfo = new UserProfileViewModel
			{
				UserId = user!.Id,
				Username = user!.UserName!,
				Bio = user!.Bio!,
				ProfilePictureUrl = user.ProfilePictureUrl,
				Followers = followers,
				Following = following,
				Posts = posts
			};

			return profileInfo;
		}

		public async Task<EditAccountInputModel> GetUserProfileEditInfoAsync(string userId)
		{
			var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userId);

			var model = new EditAccountInputModel
			{
				Username = user.UserName,
				Bio = user.Bio,
				ProfilePictureUrl = user.ProfilePictureUrl
			};

			return model;
		}

		public async Task<bool> EditUserProfileAsync(EditAccountInputModel model, string userId)
		{
			bool result = false;

			var user = _dbContext.ApplicationUsers.FirstOrDefault(u => u.Id == userId);

			if (user != null)
			{
				user.UserName = model.Username;
				user.Bio = model.Bio;
				user.ProfilePictureUrl = model.ProfilePictureUrl;

				await _dbContext.SaveChangesAsync();

				result = true;
			}

			return result;
		}

		public async Task<bool> DeleteProfileAsync(string userId)
		{
			bool result = false;

			var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userId);

			if (user != null)
			{
				_dbContext.ApplicationUsers.Remove(user);
				await _dbContext.SaveChangesAsync();

				result = true;
			}

			return result;
		}

		public async Task<bool> AddPostAsync(AddPostInputModel model, string userId)
		{
			bool result = false;

			var destination = await _dbContext.Destinations
				.FirstOrDefaultAsync(d => d.Name.ToLower() == model.DestinationName.ToLower() &&
				d.Town.ToLower() == model.DestinationTown.ToLower() &&
				d.Country.ToLower() == model.DestinationCountry.ToLower());

			if (destination == null)
			{
				destination = new Destination()
				{
					Name = model.DestinationName,
					Town = model.DestinationTown,
					Country = model.DestinationCountry
				};
			}

			var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userId);

			bool isDestinationValid = !string.IsNullOrWhiteSpace(model.DestinationName) &&
				!string.IsNullOrWhiteSpace(model.DestinationTown) &&
				!string.IsNullOrWhiteSpace(model.DestinationCountry);

			if (user != null && isDestinationValid)
			{
				var post = new Post()
				{
					Title = model.Title,
					Content = model.Content,
					ImageUrl = model.ImageUrl,
					UserId = userId,
					Destination = destination,
					CreatedOn = DateTime.UtcNow
				};

				await _dbContext.Posts.AddAsync(post);
				await _dbContext.SaveChangesAsync();

				result = true;
			}

			return result;
		}

		public async Task<EditPostInputModel> GetPostEditInfoAsync(int postId)
		{
			var model = await _dbContext.Posts
				.Where(p => p.Id == postId)
				.Include(p => p.Destination)
				.Select(p => new EditPostInputModel
				{
					PostId = p.Id,
					Title = p.Title,
					Content = p.Content,
					ImageUrl = p.ImageUrl,
					DestinationName = p.Destination.Name,
					DestinationTown = p.Destination.Town,
					DestinationCountry = p.Destination.Country
				})
				.FirstOrDefaultAsync();

			return model;
		}

		public async Task<bool> EditPostAsync(EditPostInputModel model)
		{
			bool result = false;

			var post = await _dbContext.Posts
				.Include(p => p.Destination)
				.FirstOrDefaultAsync(p => p.Id == model.PostId);

			if (post != null)
			{
				post.Title = model.Title;
				post.Content = model.Content;
				post.ImageUrl = model.ImageUrl;
				post.Destination.Name = model.DestinationName;
				post.Destination.Town = model.DestinationTown;
				post.Destination.Country = model.DestinationCountry;

				await _dbContext.SaveChangesAsync();

				result = true;
			}

			return result;
		}

		public async Task<bool> DeletePostAsync(int postId)
		{
			bool result = false;

			var post = await _dbContext.Posts
		   .Include(p => p.Comments)
		   .Include(p => p.Likes)
		   .FirstOrDefaultAsync(p => p.Id == postId);

			if (post != null)
			{
				_dbContext.Comments.RemoveRange(post.Comments);
				_dbContext.Likes.RemoveRange(post.Likes);
				_dbContext.Posts.Remove(post);

				await _dbContext.SaveChangesAsync();

				result = true;
			}

			return result;
		}

		public async Task<bool> LikePostAsync(int postId, string userId)
		{
			bool result = false;

			var post = await _dbContext.Posts
				.Include(p => p.Likes)
				.FirstOrDefaultAsync(p => p.Id == postId);

			var user = _dbContext.ApplicationUsers
				.FirstOrDefault(u => u.Id == userId);

			var existingLike = await _dbContext.Likes
				.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

			if (user != null && post != null && existingLike == null)
			{
				var like = new Like()
				{
					PostId = postId,
					UserId = userId,
					CreatedOn = DateTime.UtcNow
				};

				_dbContext.Likes.Add(like);

				post.Likes.Add(like);

				await _dbContext.SaveChangesAsync();

				result = true;
			}

			if (existingLike != null)
			{
				_dbContext.Likes.Remove(existingLike);
				await _dbContext.SaveChangesAsync();
			}

			return result;
		}

		public async Task<IEnumerable<CommentViewModel>> GetPostCommentsAsync(int postId)
		{
			var post = await _dbContext.Posts
				.Include(p => p.Comments)
				.ThenInclude(c => c.User)
				.FirstOrDefaultAsync(p => p.Id == postId);

			var commentsList = await _dbContext.Comments
				.Where(c => c.PostId == postId)
				.Select(c => new CommentViewModel
				{
					Id = c.Id,
					Content = c.Content,
					CreatedAt = c.CreatedOn,
					UserId = c.UserId,
					Username = c.User.UserName,
					UserProfilePictureUrl = c.User.ProfilePictureUrl
				})
				.OrderBy(c => c.CreatedAt)
				.ToListAsync();

			return commentsList;
		}

		public async Task<bool> AddCommentAsync(int postId, string userId, string content)
		{
			bool result = false;

			var post = await _dbContext.Posts
				.FirstOrDefaultAsync(p => p.Id == postId);

			var user = await _dbContext.ApplicationUsers
				.FirstOrDefaultAsync(u => u.Id == userId);

			bool isContentValid = content != null &&
				content.Length >= CommentContentMinLength &&
				content.Length <= CommentContentMaxLength;

			if (post != null && user != null && isContentValid)
			{
				var comment = new Comment()
				{
					Content = content!,
					PostId = postId,
					UserId = userId,
					CreatedOn = DateTime.UtcNow
				};

				await _dbContext.Comments.AddAsync(comment);
				await _dbContext.SaveChangesAsync();

				result = true;
			}

			return result;
		}

		public async Task<CommentViewModel> GetLastCommentAsync(int postId, string userId)
		{
			var latestUserComment = await _dbContext.Comments
				.Where(c => c.PostId == postId && c.UserId == userId)
				.Include(c => c.User)
				.Include(c => c.Post)
				.Select(c => new CommentViewModel
				{
					Id = c.Id,
					Content = c.Content,
					CreatedAt = c.CreatedOn,
					UserId = c.UserId,
					Username = c.User.UserName,
					UserProfilePictureUrl = c.User.ProfilePictureUrl
				})
				.OrderByDescending(c => c.CreatedAt)
				.FirstOrDefaultAsync();

			return latestUserComment;
		}

		public async Task<IEnumerable<SuggestedProfilesViewModel>> GetSuggestedProfilesInfoAsync(string userId)
		{
			var adminRoleId = _dbContext.Roles
				.Where(r => r.Name == "Administrator")
				.Select(r => r.Id)
				.FirstOrDefault();

			var adminUserIds = await _dbContext.UserRoles
				.Where(ur => ur.RoleId == adminRoleId)
				.Select(ur => ur.UserId)
				.FirstOrDefaultAsync();

			var suggestedProfiles = await _dbContext.ApplicationUsers
				.Where(u => u.Id != userId && u.Id != adminUserIds)
				.Include(u => u.Followers)
				.OrderBy(u => Guid.NewGuid())
				.Select(u => new SuggestedProfilesViewModel
				{
					UserId = u.Id,
					Username = u.UserName,
					ProfilePictureUrl = u.ProfilePictureUrl,
					Bio = u.Bio,
					IsFollowing = u.Followers.Any(f => f.FollowerId == userId)
				})
				.Where(u => u.IsFollowing == false)
				.ToListAsync();

			return suggestedProfiles;
		}

		public async Task<int?> GetPostLikesCountAsync(int postId)
		{
			var post = await _dbContext.Posts
				.Include(p => p.Likes)
				.FirstOrDefaultAsync(p => p.Id == postId);

			return post?.Likes.Count;
		}

		public async Task<IEnumerable<SuggestedPostsViewModel>> GetSuggestedPostsInfoAsync(string userId)
		{
			var textInfo = CultureInfo.InvariantCulture.TextInfo;

			var posts = await _dbContext.Posts
				.Include(p => p.User)
				.Include(p => p.Comments)
				.Include(p => p.Likes)
				.Include(p => p.Destination)
				.Where(p => p.UserId != userId)
				.Select(p => new SuggestedPostsViewModel
				{
					PostId = p.Id,
					Title = p.Title,
					ImageUrl = p.ImageUrl,
					Username = p.User.UserName,
					UserId = p.UserId,
					ShortDescription = p.Content.Length > 100
						? p.Content.Substring(0, 100) + "..."
						: p.Content,
					CommentsCount = p.Comments.Count,
					LikesCount = p.Likes.Count,
					DestinationId = p.DestinationId,
					Destination = new DestinationViewModel
					{
						Id = p.Destination.Id,
						Town = textInfo.ToTitleCase(p.Destination.Town.ToLower()),
						Country = textInfo.ToTitleCase(p.Destination.Country.ToLower())
					}
				})
				.ToListAsync();

			var rnd = new Random();
			var shuffledPosts = posts.OrderBy(p => rnd.Next()).ToList();

			return shuffledPosts;
		}

		public async Task<bool> FollowAsync(string userId, string followedUserId)
		{
			bool result = false;

			var user = await _dbContext.ApplicationUsers
				.Include(u => u.Following)
				.FirstOrDefaultAsync(u => u.Id == userId);

			var followedUser = await _dbContext.ApplicationUsers
				.Include(u => u.Followers)
				.FirstOrDefaultAsync(u => u.Id == followedUserId);

			if (user != null && followedUser != null && userId != followedUserId)
			{
				bool isAlreadyFollowing = user.Following
					.Any(f => f.FollowingId == followedUserId);

				if (!isAlreadyFollowing)
				{
					var follow = new Follow()
					{
						FollowerId = userId,
						FollowingId = followedUserId,
						CreatedOn = DateTime.UtcNow
					};

					await _dbContext.Follows.AddAsync(follow);
					await _dbContext.SaveChangesAsync();

					result = true;
				}
				else
				{
					var follow = _dbContext.Follows
						.Where(f => f.FollowerId == userId && f.FollowingId == followedUserId)
						.FirstOrDefault();

					_dbContext.Follows.Remove(follow);
					await _dbContext.SaveChangesAsync();
				}
			}

			return result;
		}

		public async Task<IEnumerable<SuggestedProfilesViewModel>> GetFollowersAsync(string userId, string currentUserId)
		{
			var user = await _dbContext.ApplicationUsers
			.Include(u => u.Followers)
			.ThenInclude(f => f.Follower)
			.FirstOrDefaultAsync(u => u.Id == userId);

			var followers = user.Followers
				.Select(f => new SuggestedProfilesViewModel
				{
					UserId = f.FollowerId,
					Username = f.Follower.UserName,
					ProfilePictureUrl = f.Follower.ProfilePictureUrl,
					Bio = f.Follower.Bio,
					IsFollowing = _dbContext.Follows.Any(x => x.FollowerId == currentUserId && x.FollowingId == f.FollowerId)
				}).ToList();

			return followers;
		}

		public async Task<IEnumerable<SuggestedProfilesViewModel>> GetFollowingAsync(string userId, string currentUserId)
		{
			var user = await _dbContext.ApplicationUsers
			.Include(u => u.Following)
			.ThenInclude(f => f.Following)
			.FirstOrDefaultAsync(u => u.Id == userId);

			var following = user.Following
				.Select(f => new SuggestedProfilesViewModel
				{
					UserId = f.FollowingId,
					Username = f.Following.UserName,
					ProfilePictureUrl = f.Following.ProfilePictureUrl,
					Bio = f.Following.Bio,
					IsFollowing = _dbContext.Follows.Any(x => x.FollowerId == currentUserId && x.FollowingId == f.FollowingId)
				}).ToList();

			return following;
		}
	}
}
