namespace Test.API.Helpers
{
	public static class GenerateSecureHashTokenOtp
	{
		public static string Generate()
		{
			return new Random().Next(1000000, 9999999).ToString();
		}
	}
}