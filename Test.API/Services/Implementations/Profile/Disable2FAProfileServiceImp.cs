using Test.API.Exceptions;
using Test.API.ExternalServices;
using Test.API.Helpers;
using Test.API.Repositories;

namespace Test.API.Services.Implementations.Profile
{
	public class Disable2FaProfileServiceImp : IDisable2FaProfileService
	{
		private readonly IAccountRepository _account;
		private readonly ITokenRepository _token;
		private readonly ITwilio _twilio;


		public Disable2FaProfileServiceImp(IAccountRepository account, ITokenRepository token, ITwilio twilio)
		{
			_account = account;
			_token = token;
			_twilio = twilio;
		}

		public async Task Disable2FaProfile(string token)
		{
			try
			{
				var validateToken = await _token.ValidateTokenAuthHeader(token, "token-signin");

				if (validateToken != null)
				{
					var account = await _account.GetByUuid(validateToken.sid);

					if (account!.TwoFactorAuth == false) throw new TwoFactorAuthNotEnabledException();

					var randomOtp = GenerateSecureHashTokenOtp.Generate();

					await _token.UpdateTokenOtp(token, randomOtp);

					await _twilio.SendConfirmationOTP(account.Phone!, randomOtp);

				}
				else
				{
					throw new TokenOtpInvalidException();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		public async Task Disable2FaConfirmationOtp(string otp, string token)
		{
			if (token == null) throw new ArgumentNullException(nameof(token));

			try
			{
				var validateToken = await _token.ValidateTokenOtp(otp, "token-signin");

				if (validateToken != null)
				{
					await _account.Disable2Fa(validateToken.Uuid);
				}
				else
				{
					throw new TokenOtpInvalidException();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				throw;
			}
		}
	}
}
