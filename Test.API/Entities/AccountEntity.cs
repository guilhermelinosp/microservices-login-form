namespace Test.API.Entities
{

	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("TB_ACCOUNT")]
	public class AccountEntity
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Key]
		[Required]
		public required string Uuid { get; set; }

		[Required]
		public required string Username { get; set; }

		[Required]
		public required string Email { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public bool ActiveAccount { get; set; }

		[Required]
		public required string? Password { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string? Phone { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public bool TwoFactorAuth { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public bool Blocked { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime CreatedAt { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime UpdatedAt { get; set; }
	}

}
