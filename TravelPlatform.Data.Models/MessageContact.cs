using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static TravelPlatform.GCommon.ValidationConstants.MessageContactValidationConstants;

namespace TravelPlatform.Data.Models
{
	public class MessageContact
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(NamesMaxLength)]
		public string FirstName { get; set; } = null!;

		[Required]
		[MaxLength(NamesMaxLength)]
		public string LastName { get; set; } = null!;

		[Required]
		public string Email { get; set; } = null!;

		[Required]
		[MaxLength(SubjectMaxLength)]
		public string Subject { get; set; } = null!;

		[Required]
		[MaxLength(ContentMaxLength)]
		public string Content { get; set; } = null!;

		public DateTime SentAt { get; set; }

		[DefaultValue(false)]
		public bool IsRead { get; set; }
	}
}
