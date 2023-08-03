using System.ComponentModel.DataAnnotations;

namespace Test.API.Dtos.Account
{
	public class ForgotPasswordAccountDto
	{
		[Required]
		public required string Email { get; set; }
	}
}
