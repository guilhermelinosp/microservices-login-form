using Test.API.Dtos.Account;
using Test.API.Entities;
using Test.API.Exceptions;
using Test.API.ExternalServices;
using Test.API.Helpers;
using Test.API.Repositories;
using static System.Text.RegularExpressions.Regex;

namespace Test.API.Services.Implementations.Account
{
	public class SignUpAccountServiceImp : ISignUpAccountService
	{
		private readonly IAccountRepository _account;
		private readonly ITokenRepository _token;
		private readonly ISendGrid _sendGrid;
		public SignUpAccountServiceImp(IAccountRepository account, ITokenRepository token, ISendGrid sendGrid)
		{
			_account = account;
			_token = token;
			_sendGrid = sendGrid;
		}

		public async Task SignUpAccount(SignUpAccountDto? account)
		{
			if (account == null) throw new ArgumentNullException(nameof(account));

			try
			{
				var existingAccount = await _account.GetByEmail(account.Email);
				if (existingAccount != null) throw new AccountNotFoundException();
				if (!IsMatch(account.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")) throw new AccountEmailInvalidException();
				if (account.Password.Length < 8) throw new AccountInvalidCredentialsException();
				if (account.Password != account.ConfirmPassword) throw new AccountInvalidCredentialsException();

				var accountEntity = new AccountEntity
				{
					Uuid = Guid.NewGuid().ToString(),
					Username = GenerateSecureHashUsername.Generate(account.Email),
					Email = account.Email,
					Password = GenerateSecureHashPassword.Generate(account.Password)
				};

				var randomOtp = GenerateSecureHashTokenOtp.Generate();

				await _account.CreateAccount(accountEntity);

				await _token.GenerateToken(accountEntity.Uuid, "token-signup", randomOtp);

				await _sendGrid.SendSignUpAccountActivation(accountEntity.Email, randomOtp);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				throw;
			}
		}

		public async Task SignUpAccountActivation(string token)
		{
			try
			{
				if (token == null) throw new ArgumentNullException(nameof(token));

				var validateToken = await _token.ValidateTokenOtp(token, "token-signup");

				if (validateToken == null)
				{
					throw new TokenOtpInvalidException();
				}
				else
				{
					await _account.ActivateAccount(validateToken.Uuid);
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
