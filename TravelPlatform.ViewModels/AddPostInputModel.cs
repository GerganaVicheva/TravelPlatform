using System.ComponentModel.DataAnnotations;
using static TravelPlatform.GCommon.ValidationConstants.PostValidationConstants;
using static TravelPlatform.GCommon.ValidationConstants.DestinationValidationConstants;

namespace TravelPlatform.ViewModels
{
    public class AddPostInputModel
    {
		[Required]
		[MaxLength(PostTitleMaxLength)]
		[MinLength(PostTitleMinLength)]
		public string Title { get; set; } = null!;

		[MaxLength(PostContentMaxLength)]
		public string? Content { get; set; }

		public string? ImageUrl { get; set; }

		[Required]
		[MaxLength(DestinationNameMaxLength)]
		[MinLength(DestinationNameMinLength)]
		public string DestinationName { get; set; } = null!;

		[Required]
		[MaxLength(DestinationTownMaxLength)]
		[MinLength(DestinationTownMinLength)]
		public string DestinationTown { get; set; } = null!;

		[Required]
		[MaxLength(DestinationCountryMaxLength)]
		[MinLength(DestinationCountryMinLength)]
		public string DestinationCountry { get; set; } = null!;
	}
}
