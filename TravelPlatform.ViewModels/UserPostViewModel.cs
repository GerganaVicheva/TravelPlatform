namespace TravelPlatform.ViewModels
{
    public class UserPostViewModel
    {
		public int Id { get; set; }

		public string Title { get; set; }

		public string? Content { get; set; }

		public string? ImageUrl { get; set; }

		public string DestinationName { get; set; } 

		public string Town { get; set; } 

		public string Country { get; set; } 

		public DateTime CreatedOn { get; set; }

		public int Likes { get; set; }

		public int Comments { get; set; }

		public bool IsLikedByCurrentUser { get; set; }
	}
}
