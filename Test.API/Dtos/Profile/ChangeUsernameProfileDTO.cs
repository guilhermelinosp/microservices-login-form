using System.ComponentModel.DataAnnotations;

namespace Test.API.Dtos.Profile
{
	public class ChangeUsernameProfileDto
	{
		[Required]
		public required string Username { get; set; }
	}
}
