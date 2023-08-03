namespace Test.API.ExternalServices
{
	public interface ISendGrid
	{
		Task SendForgotPassword(string sendEmail, string token);

		Task SendSignUpAccountActivation(string sendEmail, string otp);

		Task SendConfirmationOTP(string sendEmail, string token);
	}
}
