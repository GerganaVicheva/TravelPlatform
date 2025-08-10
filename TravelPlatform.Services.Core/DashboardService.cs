using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System;
using TravelPlatform.Services.Core.Contracts;
using TravelPlatform.ViewModels;
using TravelPlatform.Web.Data;

namespace TravelPlatform.Services.Core
{
	public class DashboardService : IDashboardService
	{
		private readonly TravelPlatformDbContext _dbContext;

		public DashboardService(TravelPlatformDbContext context)
		{
			_dbContext = context;
		}

		public async Task<(string[] Days, int[] TravelPosts, int[] DailyActiveUsers)> GetDashboardDataAsync()
		{
			var today = DateTime.UtcNow.Date;
			var startDate = today.AddDays(-6); // last 7 days including today

			var days = Enumerable.Range(0, 7)
				.Select(offset => startDate.AddDays(offset).ToString("ddd"))
				.ToArray();

			var travelPosts = new int[7];
			var dailyActiveUsers = new int[7];

			for (int i = 0; i < 7; i++)
			{
				var date = startDate.AddDays(i);

				travelPosts[i] = await _dbContext.Posts
					.Where(p => p.CreatedOn.Date == date)
					.CountAsync();

				dailyActiveUsers[i] = await GetDailyActiveUserCountAsync(date);
			}

			return (days, travelPosts, dailyActiveUsers);
		}

		private async Task<int> GetDailyActiveUserCountAsync(DateTime date)
		{
			var postUserIds = await _dbContext.Posts
				.Where(p => p.CreatedOn.Date == date)
				.Select(p => p.UserId)
				.ToListAsync();

			var commentUserIds = await _dbContext.Comments
				.Where(c => c.CreatedOn.Date == date)
				.Select(c => c.UserId)
				.ToListAsync();

			var likeUserIds = await _dbContext.Likes
				.Where(l => l.CreatedOn.Date == date)
				.Select(l => l.UserId)
				.ToListAsync();

			return postUserIds
				.Concat(commentUserIds)
				.Concat(likeUserIds)
				.Distinct()
				.Count();
		}

		public async Task<DashboardDataViewModel> GetDashboardTotalDataAsync()
		{
			var dahboardData = new DashboardDataViewModel
			{
				TotalUsers = await _dbContext.Users.CountAsync(),
				TotalPosts = await _dbContext.Posts.CountAsync(),
				TotalLikes = await _dbContext.Likes.CountAsync(),
				TotalComments = await _dbContext.Comments.CountAsync()
			};

			return dahboardData;
		}
	}
}
