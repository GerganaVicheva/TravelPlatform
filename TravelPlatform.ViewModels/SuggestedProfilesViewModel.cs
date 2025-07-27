namespace TravelPlatform.ViewModels
{
    public class SuggestedProfilesViewModel
    {
		public string UserId { get; set; }

		public string Username { get; set; }

		public string? ProfilePictureUrl { get; set; }

		public string? Bio { get; set; }

		public bool IsFollowing { get; set; }
	}
}
