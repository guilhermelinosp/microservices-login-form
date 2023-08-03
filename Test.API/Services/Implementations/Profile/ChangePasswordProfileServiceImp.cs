using Test.API.Dtos.Profile;
using Test.API.Exceptions;
using Test.API.Helpers;
using Test.API.Repositories;

namespace Test.API.Services.Implementations.Profile
{
	public class ChangePasswordProfileServiceImp : IChangePasswordProfileService
	{
		private readonly IAccountRepository _account;
		private readonly ITokenRepository _token;

		public ChangePasswordProfileServiceImp(IAccountRepository account, ITokenRepository token)
		{
			_account = account;
			_token = token;
		}

		public async Task ChangePasswordProfile(ChangePasswordProfileDto changePasswordModel, string token)
		{
			if (changePasswordModel == null) throw new ArgumentNullException(nameof(changePasswordModel));
			if (token == null) throw new ArgumentNullException(nameof(token));

			try
			{
				var validateToken = await _token.ValidateTokenAuthHeader(token, "token-signin");

				if (validateToken != null)
				{
					var account = await _account.GetByUuid(validateToken.sid);

					var hashLastPassword = GenerateSecureHashPassword.Generate(changePasswordModel.LastPassword);

					if (account!.Password != hashLastPassword) throw new AccountInvalidCredentialsException();

					if (changePasswordModel.Password != changePasswordModel.ConfirmPassword)
						throw new AccountInvalidCredentialsException();

					if (changePasswordModel.Password.Length < 8) throw new AccountInvalidCredentialsException();

					var hashNewPassword = GenerateSecureHashPassword.Generate(changePasswordModel.Password);

					await _account.ChangePassword(validateToken.sid, hashNewPassword);
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
