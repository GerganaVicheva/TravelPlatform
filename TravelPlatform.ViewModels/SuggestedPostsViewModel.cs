namespace TravelPlatform.ViewModels
{
    public class SuggestedPostsViewModel
    {
		public string UserId { get; set; }

		public int PostId { get; set; }

		public string Title { get; set; }

		public string ImageUrl { get; set; }

		public string Username { get; set; }

		public string ShortDescription { get; set; }

		public int LikesCount { get; set; }

		public int CommentsCount { get; set; }

		public int DestinationId { get; set; }

		public DestinationViewModel Destination { get; set; }
	}
}
