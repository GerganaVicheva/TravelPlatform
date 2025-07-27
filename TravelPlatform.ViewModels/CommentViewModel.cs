namespace TravelPlatform.ViewModels
{
    public class CommentViewModel
    {
		public int Id { get; set; }

		public string Content { get; set; }

		public DateTime CreatedAt { get; set; }

		public string UserId { get; set; } 

		public string Username { get; set; } 

		public string? UserProfilePictureUrl { get; set; }
	}
}
