using SendGrid;
using SendGrid.Helpers.Mail;

namespace Test.API.ExternalServices.Implementations
{
	public class SendGridImp : ISendGrid
	{
		private readonly IConfiguration _configuration;

		public SendGridImp(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendForgotPassword(string sendEmail, string token)
		{
			var sendGridClient = new SendGridClient(_configuration["SendGrid:ApiKey"]!);
			var from = new EmailAddress(_configuration["SendGrid:FromEmail"]!, _configuration["SendGrid:FromName"]!);
			var to = new EmailAddress(sendEmail);
			const string subject = "Test Forgot Password";
			const string plainTextContent = "Test Forgot Password";
			var htmlContent = $"<p>Hi there!</p><p>It's okay to forget your password</p><p>To <span>confirm</span> your <span>email</span>, simply go to: <a href=\"http://localhost:5014/api/private/account/changepassword?token={token}\" target=\"_blank\" data-saferedirecturl=\"https://www.google.com/url?q=http://localhost:5014/api/private/account/changepassword?token={token}amp;source=gmail&amp;ust=1689764401795000&amp;usg=AOvVaw3xy_-1KkjdXR5woYXt1WC7\">http://localhost:5014/api/private/account/<wbr>changepassword?token={token}<wbr></a></p><p>The Team Test API</p>";

			await sendGridClient.SendEmailAsync(MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent));
		}

		public async Task SendSignUpAccountActivation(string sendEmail, string token)
		{
			var sendGridClient = new SendGridClient(_configuration["SendGrid:ApiKey"]!);
			var from = new EmailAddress(_configuration["SendGrid:FromEmail"]!, _configuration["SendGrid:FromName"]!);
			var to = new EmailAddress(sendEmail);
			const string subject = "Test Account Activation";
			const string plainTextContent = "Test Account Activation";
			var htmlContent =
				$"<p>Hi there!</p><p>Thanks for signing up in Test API!</p><p>To <span>confirm</span> your <span>email</span>, simply go to: <a href=\"https://localhost:8081/api/public/account/signup/otp?token={token}\" target=\"_blank\" data-saferedirecturl=\"https://www.google.com/url?q=https://localhost:8081/api/public/account/signup/otp?token={token}amp;source=gmail&amp;ust=1689764401795000&amp;usg=AOvVaw3xy_-1KkjdXR5woYXt1WC7\">https://localhost:8081/api/public/account/<wbr>signup/otp?token={token}<wbr></a></p><p>The Team Test API</p>";


			await sendGridClient.SendEmailAsync(MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent));
		}

		public async Task SendConfirmationOTP(string sendEmail, string token)
		{
			var sendGridClient = new SendGridClient(_configuration["SendGrid:ApiKey"]!);
			var from = new EmailAddress(_configuration["SendGrid:FromEmail"]!, _configuration["SendGrid:FromName"]!);
			var to = new EmailAddress(sendEmail);
			const string subject = "Test Confirmation OTP";
			const string plainTextContent = "Test Confirmation OTP";
			var htmlContent = "<p>Hi there!</p>" + $"Your security code for Test API: {token}" + "<p>The Team Test API</p>";

			await sendGridClient.SendEmailAsync(MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent));
		}
	}
}