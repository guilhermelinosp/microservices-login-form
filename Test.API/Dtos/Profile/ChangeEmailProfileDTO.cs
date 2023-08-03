using System.ComponentModel.DataAnnotations;

namespace Test.API.Dtos.Profile
{
	public class ChangeEmailProfileDto
	{
		[Required]
		public required string NewEmail { get; set; }
	}
}
