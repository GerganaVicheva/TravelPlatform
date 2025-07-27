using System.ComponentModel.DataAnnotations;

namespace TravelPlatform.ViewModels
{
    public class EditAccountInputModel
    {
		[Required]
		public string Username { get; set; }

		public string? Bio { get; set; }

		public string? ProfilePictureUrl { get; set; }
	}
}
