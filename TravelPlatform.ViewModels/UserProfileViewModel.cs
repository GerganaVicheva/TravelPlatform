namespace TravelPlatform.ViewModels
{
    public class UserProfileViewModel
    {
		public string UserId { get; set; }

		public string Username { get; set; }

		public string Bio { get; set; }

		public string ProfilePictureUrl { get; set; }

		public int PostCount { get; set; }

		public List<SuggestedProfilesViewModel> Followers { get; set; } = new();

		public List<SuggestedProfilesViewModel> Following { get; set; } = new();

		public List<UserPostViewModel> Posts { get; set; } = new();
	}
}
