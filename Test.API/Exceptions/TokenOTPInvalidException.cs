namespace Test.API.Exceptions
{
	public class TokenOtpInvalidException : Exception
	{
		public TokenOtpInvalidException() : base("Invalid token. Please check your OTP.")
		{
		}

		public TokenOtpInvalidException(string message) : base(message)
		{
		}

		public TokenOtpInvalidException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}

}
