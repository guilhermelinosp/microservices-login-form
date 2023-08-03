namespace Test.API.Entities
{
	public class TokenPayload
	{
		public required string sub { get; set; }
		public required string typ { get; set; }
		public required string sid { get; set; }
		public required string nbf { get; set; }
		public required string exp { get; set; }
		public required string iat { get; set; }
		public required string iss { get; set; }
		public required string aud { get; set; }
	}
}
