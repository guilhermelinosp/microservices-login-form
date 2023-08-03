using Test.API.Exceptions;
using Test.API.ExternalServices;
using Test.API.Helpers;
using Test.API.Repositories;

namespace Test.API.Services.Implementations.Profile
{
	public class DeactivateAccountImp : IDeactivateAccount
	{
		private readonly IAccountRepository _account;
		private readonly ITokenRepository _token;
		private readonly ITwilio _twilio;
		private readonly ISendGrid _sendGrid;



		public DeactivateAccountImp(IAccountRepository account, ITokenRepository token, ITwilio twilio, ISendGrid sendGrid)
		{
			_account = account;
			_token = token;
			_twilio = twilio;
			_sendGrid = sendGrid;
		}
		public async Task DeactivateAccount(string token)
		{
			try
			{
				var validateToken = await _token.ValidateTokenAuthHeader(token, "token-signin");

				if (validateToken != null)
				{
					var account = await _account.GetByUuid(validateToken.sid);

					var randomOtp = GenerateSecureHashTokenOtp.Generate();

					await _token.UpdateTokenOtp(token, randomOtp);

					if (account!.TwoFactorAuth == false)
					{
						await _sendGrid.SendConfirmationOTP(account.Email, randomOtp);
					}
					else
					{
						await _twilio.SendConfirmationOTP(account.Phone!, randomOtp);
					}
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

		public async Task DeactivateAccountConfirmationOtp(string otp, string token)
		{
			if (token == null) throw new ArgumentNullException(nameof(token));

			try
			{
				var validateToken = await _token.ValidateTokenOtp(otp, "token-signin");

				if (validateToken != null)
				{
					await _account.DeactivateAccount(validateToken.Uuid);
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
