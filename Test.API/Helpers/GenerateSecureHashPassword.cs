using System.Text;
using static System.Security.Cryptography.SHA256;

namespace Test.API.Helpers
{
	public static class GenerateSecureHashPassword
	{
		public static string Generate(string password)
		{
			return Convert.ToBase64String(Create().ComputeHash(Encoding.UTF8.GetBytes(password)));
		}
	}
}
