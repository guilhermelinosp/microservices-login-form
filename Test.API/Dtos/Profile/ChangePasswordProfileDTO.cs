using System.ComponentModel.DataAnnotations;

namespace Test.API.Dtos.Profile
{
	public class ChangePasswordProfileDto
	{
		[Required]
		public required string LastPassword { get; set; }

		[Required]
		public required string Password { get; set; }

		[Required]
		public required string ConfirmPassword { get; set; }
	}
}
