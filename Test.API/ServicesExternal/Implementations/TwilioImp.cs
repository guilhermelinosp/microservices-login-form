using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Test.API.ExternalServices.Implementations
{
	public class TwilioImp : ITwilio
	{
		private readonly IConfiguration _configuration;

		public TwilioImp(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendConfirmationOTP(string phoneNumber, string token)
		{
			TwilioClient.Init(_configuration["Twilio:AccountSID"]!, _configuration["Twilio:AuthToken"]!);

			await MessageResource.CreateAsync(
				body: $"Your security code for Test API: {token}",
				from: new PhoneNumber(_configuration["Twilio:FromNumber"]!),
				to: new PhoneNumber(phoneNumber)
			);
		}
	}
}