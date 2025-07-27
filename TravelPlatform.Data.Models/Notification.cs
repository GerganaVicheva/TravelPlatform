using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelPlatform.GCommon;
using static TravelPlatform.GCommon.ValidationConstants.NotificationValidationConstants;

namespace TravelPlatform.Data.Models
{
    public class Notification
    {
		[Key]
		public int NotificationId { get; set; }

		[ForeignKey(nameof(User))]
		public string UserId { get; set; } = null!;

		public ApplicationUser User { get; set; } = null!;

		[ForeignKey(nameof(FromUser))]
		public string? FromUserId { get; set; }

		public ApplicationUser? FromUser { get; set; }

		[ForeignKey(nameof(Post))]
		public int? PostId { get; set; }

		public Post? Post { get; set; }

		[ForeignKey(nameof(Comment))]
		public int? CommentId { get; set; }

		public Comment? Comment { get; set; }

		public NotificationType Type { get; set; }

		[MaxLength(NotificationMessageMaxLength)]
		public string Message { get; set; } = null!;

		[DefaultValue(false)]
		public bool IsRead { get; set; }

		public DateTime CreatedOn { get; set; }
	}
}
