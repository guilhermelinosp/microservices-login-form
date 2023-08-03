using Test.API.Dtos;
using Test.API.Dtos.Account;

namespace Test.API.Services
{
	public interface ISignInAccountService
	{
		Task SignInAccount(SignInAccountDto account);
		Task<TokenDto?> SignInAccountConfirmationOtp(string otp);
	}
}
