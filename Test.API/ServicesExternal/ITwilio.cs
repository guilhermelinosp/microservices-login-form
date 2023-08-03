namespace Test.API.ExternalServices
{
	public interface ITwilio
	{
		Task SendConfirmationOTP(string phoneNumber, string token);
	}
}
