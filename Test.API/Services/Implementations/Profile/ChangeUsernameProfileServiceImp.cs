using Test.API.Dtos.Profile;
using Test.API.Exceptions;
using Test.API.Repositories;

namespace Test.API.Services.Implementations.Profile
{
	public class ChangeUsernameProfileServiceImp : IChangeUsernameProfileService
	{
		private readonly IAccountRepository _account;
		private readonly ITokenRepository _token;

		public ChangeUsernameProfileServiceImp(IAccountRepository account, ITokenRepository token)
		{
			_account = account;
			_token = token;
		}

		public async Task ChangeUsernameProfile(ChangeUsernameProfileDto username, string token)
		{
			if (username == null) throw new ArgumentNullException(nameof(username));
			if (token == null) throw new ArgumentNullException(nameof(token));

			try
			{
				var validateToken = await _token.ValidateTokenAuthHeader(token, "token-signin");

				if (validateToken != null)
				{
					var existingUsername = _account.GetByUsername(username.Username);

					if (existingUsername == null) throw new AccountUsernameAlreadyExistsException();

					await _account.ChangeUsername(validateToken.sid, username.Username);
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
