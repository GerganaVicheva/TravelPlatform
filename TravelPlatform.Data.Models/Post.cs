using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static TravelPlatform.GCommon.ValidationConstants.PostValidationConstants;

namespace TravelPlatform.Data.Models
{
    public class Post
    {
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(PostTitleMaxLength)]
		public string Title { get; set; } = null!;

		[MaxLength(PostContentMaxLength)]
		public string? Content { get; set; }

		public string? ImageUrl { get; set; }

		public DateTime CreatedOn { get; set; }

		[ForeignKey(nameof(User))]
		public string UserId { get; set; } = null!;

		public ApplicationUser User { get; set; } = null!;

		[ForeignKey(nameof(Destination))]
		public int DestinationId { get; set; }

		public Destination Destination { get; set; } = null!;

		public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

		public ICollection<Like> Likes { get; set; } = new HashSet<Like>();
	}
}
