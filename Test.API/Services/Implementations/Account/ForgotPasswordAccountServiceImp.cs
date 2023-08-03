using Test.API.Dtos;
using Test.API.Exceptions;
using Test.API.ExternalServices;
using Test.API.Helpers;
using Test.API.Repositories;

namespace Test.API.Services.Implementations.Account
{
	public class ForgotPasswordAccountServiceImp : IForgotPasswordAccountService
	{
		private readonly IAccountRepository _account;
		private readonly ITokenRepository _token;
		private readonly ISendGrid _sendGrid;
		private readonly ITwilio _twilio;

		public ForgotPasswordAccountServiceImp(IAccountRepository account, ITokenRepository token, ISendGrid sendGrid, ITwilio twilio)
		{
			_account = account;
			_token = token;
			_sendGrid = sendGrid;
			_twilio = twilio;
		}

		public async Task ForgotPasswordAccount(string email)
		{
			if (email == null) throw new ArgumentNullException(nameof(email));

			try
			{
				var existingAccount = await _account.GetByEmail(email);

				if (existingAccount != null)
				{
					var randomOtp = GenerateSecureHashTokenOtp.Generate();

					await _token.GenerateToken(existingAccount.Uuid, "token-forgotpassword", randomOtp);

					if (existingAccount.TwoFactorAuth)
					{
						await _twilio.SendConfirmationOTP(existingAccount.Phone!, randomOtp);
					}
					else
					{
						await _sendGrid.SendConfirmationOTP(existingAccount.Email, randomOtp);
					}
				}
				else
				{
					throw new AccountNotFoundException();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				throw;
			}
		}

		public async Task<TokenDto?> ForgotPasswordConfirmationOtp(string token)
		{
			if (token == null) throw new ArgumentNullException(nameof(token));
			try
			{
				var validateToken = await _token.ValidateTokenOtp(token, "token-forgotpassword");

				if (validateToken != null)
				{
					return new TokenDto
					{
						Token = validateToken.Token
					};
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
