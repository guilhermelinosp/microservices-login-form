using Test.API.Dtos;
using Test.API.Dtos.Account;
using Test.API.Entities;
using Test.API.Exceptions;
using Test.API.ExternalServices;
using Test.API.Helpers;
using Test.API.Repositories;
using static System.Text.RegularExpressions.Regex;

namespace Test.API.Services.Implementations.Account
{
	public class SignInAccountServiceImp : ISignInAccountService
	{
		private readonly IAccountRepository _account;
		private readonly ITokenRepository _token;
		private readonly ITwilio _twilio;
		private readonly ISendGrid _sendGrid;


		public SignInAccountServiceImp(IAccountRepository account, ITokenRepository token, ITwilio twilio, ISendGrid sendGrid)
		{
			_account = account;
			_token = token;
			_twilio = twilio;
			_sendGrid = sendGrid;
		}

		public async Task SignInAccount(SignInAccountDto account)
		{
			if (account == null) throw new ArgumentNullException(nameof(account));

			try
			{
				AccountEntity? existingAccount;

				if (IsMatch(account.Email_or_username, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
				{
					existingAccount = await _account.GetByEmail(account.Email_or_username);
				}
				else
				{
					existingAccount = await _account.GetByUsername(account.Email_or_username);
				}

				if (existingAccount != null)
				{
					if (existingAccount.Blocked) throw new AccountBlockedException();
					if (existingAccount.ActiveAccount == false) throw new AccountNotActivatedException();

					var hashedPassword = GenerateSecureHashPassword.Generate(account.Password);

					if (hashedPassword != existingAccount.Password) throw new AccountInvalidCredentialsException();

					var randomOtp = GenerateSecureHashTokenOtp.Generate();


					await _token.GenerateToken(existingAccount.Uuid, "token-signin", randomOtp);

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

		public async Task<TokenDto?> SignInAccountConfirmationOtp(string token)
		{
			if (token == null) throw new ArgumentNullException(nameof(token));

			try
			{
				var validateToken = await _token.ValidateTokenOtp(token, "token-signin");

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
