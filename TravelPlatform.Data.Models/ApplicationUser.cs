using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static TravelPlatform.GCommon.ValidationConstants.UserValidationConstants;

namespace TravelPlatform.Data.Models
{
    public class ApplicationUser : IdentityUser
	{
		[MaxLength(BioMaxLength)]
        public string? Bio { get; set; }

		public string? ProfilePictureUrl { get; set; }

		public ICollection<Follow> Followers { get; set; } = new HashSet<Follow>();
		public ICollection<Follow> Following { get; set; } = new HashSet<Follow>();
	}
}
