using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlatform.Data.Models
{
    public class Follow
    {
		public DateTime CreatedOn { get; set; }

		[ForeignKey(nameof(Follower))]
		public string FollowerId { get; set; } = null!;

		public ApplicationUser Follower { get; set; } = null!;

		[ForeignKey(nameof(Following))]
		public string FollowingId { get; set; } = null!;

		public ApplicationUser Following { get; set; } = null!;
	}
}
