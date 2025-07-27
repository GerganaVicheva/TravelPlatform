using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static TravelPlatform.GCommon.ValidationConstants.CommentValidationConstants;

namespace TravelPlatform.Data.Models
{
    public class Comment
    {
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(CommentContentMaxLength)]
		public string Content { get; set; } = null!;

		public DateTime CreatedOn { get; set; }

		[ForeignKey(nameof(Post))]
		public int PostId { get; set; }

		public Post Post { get; set; } = null!;

		[ForeignKey(nameof(User))]
		public string UserId { get; set; } = null!;

		public ApplicationUser User { get; set; } = null!;
	}
}
