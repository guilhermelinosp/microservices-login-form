using Test.API.Exceptions;
using Test.API.ExternalServices;
using Test.API.Helpers;
using Test.API.Repositories;

namespace Test.API.Services.Implementations.Profile
{
	public class Enable2FaProfileServiceImp : IEnable2FAProfileServices
	{
		private readonly IAccountRepository _account;
		private readonly ITokenRepository _token;
		private readonly ITwilio _twilio;


		public Enable2FaProfileServiceImp(IAccountRepository account, ITokenRepository token, ITwilio twilio)
		{
			_account = account;
			_token = token;
			_twilio = twilio;
		}

		public async Task Enable2FAProfile(string phone, string token)
		{
			if (phone == null) throw new ArgumentNullException(nameof(phone));

			try
			{
				var validateToken = await _token.ValidateTokenAuthHeader(token, "token-signin");

				if (validateToken != null)
				{
					var randomOtp = GenerateSecureHashTokenOtp.Generate();

					await _token.UpdateTokenOtp(token, randomOtp);

					await _twilio.SendConfirmationOTP(phone, randomOtp);

					await _account.ChangePhone(validateToken.sid, phone);
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

		public async Task Enable2FAConfirmationOTP(string otp, string token)
		{
			if (token == null) throw new ArgumentNullException(nameof(token));

			try
			{
				var validateToken = await _token.ValidateTokenOtp(otp, "token-signin");

				if (validateToken == null) throw new TokenOtpInvalidException();

				await _account.Enable2Fa(validateToken.Uuid);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				throw;
			}
		}
	}
}
