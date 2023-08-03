using System.ComponentModel.DataAnnotations;

namespace Test.API.Dtos.Account
{
	public class SignInAccountDto
	{
		[Required]
		public required string Email_or_username { get; set; }

		[Required]
		public required string Password { get; set; }
	}
}
