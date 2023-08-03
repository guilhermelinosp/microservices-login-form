namespace Test.API.Services
{
	public interface IEnable2FAProfileServices
	{
		Task Enable2FAProfile(string phone, string token);

		Task Enable2FAConfirmationOTP(string otp, string token);
	}
}
