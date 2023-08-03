using System.ComponentModel.DataAnnotations;

namespace Test.API.Dtos.Account
{
	public class SignUpAccountDto
	{
		[Required]
		public required string Email { get; set; }

		[Required]
		public required string Password { get; set; }

		[Required]
		public required string ConfirmPassword { get; set; }
	}
}
