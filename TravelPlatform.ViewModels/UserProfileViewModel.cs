namespace TravelPlatform.ViewModels
{
    public class UserProfileViewModel
    {
		public string UserId { get; set; }

		public string Username { get; set; }

		public string Bio { get; set; }

		public string ProfilePictureUrl { get; set; }

		public int PostCount { get; set; }

		public int Followers { get; set; }

		public int Following { get; set; }

		public List<UserPostViewModel> Posts { get; set; } = new();
	}
}
