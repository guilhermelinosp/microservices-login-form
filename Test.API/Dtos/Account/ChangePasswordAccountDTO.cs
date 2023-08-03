using System.ComponentModel.DataAnnotations;

namespace Test.API.Dtos.Account
{
	public class ChangePasswordAccountDto
	{
		[Required]
		public required string Password { get; set; }

		[Required]
		public required string ConfirmPassword { get; set; }
	}
}
