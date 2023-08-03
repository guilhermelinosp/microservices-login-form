using System.ComponentModel.DataAnnotations;

namespace Test.API.Dtos.Profile
{
	public class Enable2FaProfileDto
	{
		[Required]
		public required string Phone { get; set; }
	}
}
