using System.ComponentModel.DataAnnotations;
using static TravelPlatform.GCommon.ValidationConstants.MessageContactValidationConstants;

namespace TravelPlatform.ViewModels
{
    public class AddMessageInputModel
    {
		[Required]
		[MaxLength(NamesMaxLength)]
		[MinLength(NamesMinLength)]
		public string FirstName { get; set; } = null!;

		[Required]
		[MaxLength(NamesMaxLength)]
		[MinLength(NamesMinLength)]
		public string LastName { get; set; } = null!;

		[Required]
		public string Email { get; set; } = null!;

		[Required]
		[MaxLength(SubjectMaxLength)]
		[MinLength(SubjectMinLength)]
		public string Subject { get; set; } = null!;

		[Required]
		[MaxLength(ContentMaxLength)]
		[MinLength(ContentMinLength)]
		public string Content { get; set; } = null!;
	}
}
