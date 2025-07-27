using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlatform.Data.Models
{
    public class Like
    {
		public DateTime CreatedOn { get; set; }

		[ForeignKey(nameof(Post))]
		public int PostId { get; set; }

		public Post Post { get; set; } = null!;

		[ForeignKey(nameof(User))]
		public string UserId { get; set; } = null!;

		public ApplicationUser User { get; set; } = null!;
	}
}
