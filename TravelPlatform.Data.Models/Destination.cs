using System.ComponentModel.DataAnnotations;
using static TravelPlatform.GCommon.ValidationConstants.DestinationValidationConstants;

namespace TravelPlatform.Data.Models
{
    public class Destination
    {
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(DestinationNameMaxLength)]
		public string Name { get; set; } = null!;

		[Required]
		[MaxLength(DestinationTownMaxLength)]
		public string Town { get; set; } = null!;

		[Required]
		[MaxLength(DestinationCountryMaxLength)]
		public string Country { get; set; } = null!;

		public ICollection<Post> Posts { get; set; } = new HashSet<Post>();
	}
}
