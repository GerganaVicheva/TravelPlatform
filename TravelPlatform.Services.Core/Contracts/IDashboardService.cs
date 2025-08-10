using TravelPlatform.ViewModels;

namespace TravelPlatform.Services.Core.Contracts
{
    public interface IDashboardService
    {
		Task<(string[] Days, int[] TravelPosts, int[] DailyActiveUsers)> GetDashboardDataAsync();

		Task<DashboardDataViewModel> GetDashboardTotalDataAsync();
	}
}
