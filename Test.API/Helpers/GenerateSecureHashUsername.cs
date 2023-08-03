using static System.Text.RegularExpressions.Regex;

namespace Test.API.Helpers
{
	public static class GenerateSecureHashUsername
	{
		public static string Generate(string email)
		{
			return Replace(email.Substring(0, email.LastIndexOf("@", StringComparison.Ordinal)), @"[^a-zA-Z0-9]", "").Replace("a", "4").Replace("i", "1").Replace("e", "3").Replace("o", "0");
		}
	}
}
