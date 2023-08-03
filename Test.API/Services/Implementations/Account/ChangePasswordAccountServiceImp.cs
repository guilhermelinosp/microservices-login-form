using Test.API.Dtos.Account;
using Test.API.Exceptions;
using Test.API.Helpers;
using Test.API.Repositories;

namespace Test.API.Services.Implementations.Account
{
	public class ChangePasswordAccountServiceImp : IChangePasswordAccountService
	{
		private readonly IAccountRepository _account;
		private readonly ITokenRepository _token;

		public ChangePasswordAccountServiceImp(IAccountRepository account, ITokenRepository token)
		{
			_account = account;
			_token = token;
		}

		public async Task ChangePasswordAccount(ChangePasswordAccountDto password, string token)
		{
			if (password == null) throw new ArgumentNullException(nameof(password));

			try
			{
				var tokenPayload = await _token.ValidateTokenForgotPassword(token, "token-forgotpassword");

				if (tokenPayload != null)
				{
					if (password.Password != password.ConfirmPassword) throw new AccountEmailInvalidException();

					if (password.Password.Length < 8) throw new AccountEmailInvalidException();

					var hashNewPassword = GenerateSecureHashPassword.Generate(password.Password);

					await _account.ChangePassword(tokenPayload.Uuid, hashNewPassword);
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
