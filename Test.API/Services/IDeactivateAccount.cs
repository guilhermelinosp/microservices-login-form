namespace Test.API.Services
{
	public interface IDeactivateAccount
	{
		Task DeactivateAccount(string token);
		Task DeactivateAccountConfirmationOtp(string otp, string token);
	}
}
