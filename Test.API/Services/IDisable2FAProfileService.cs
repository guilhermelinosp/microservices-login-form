namespace Test.API.Services
{
	public interface IDisable2FaProfileService
	{
		Task Disable2FaProfile(string token);
		Task Disable2FaConfirmationOtp(string otp, string token);

	}
}
