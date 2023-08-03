using System.ComponentModel.DataAnnotations;

namespace Test.API.Dtos
{
	public class TokenDto
	{
		[Required]
		public required string Token { get; set; }
	}
}
