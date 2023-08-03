using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Test.API.Entities
{
	public class TokenEntity
	{
		[BsonId]
		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string? Id { get; set; }

		[Required]
		public required string Token { get; set; }

		[Required]
		public required string Otp { get; set; }

		[Required]
		public required string Type { get; set; }

		[Required]
		public required string Uuid { get; set; }

		[Required]
		public DateTime ExpiresAt { get; set; }
	}
}
