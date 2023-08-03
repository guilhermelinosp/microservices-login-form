using Test.API.Dtos;

namespace Test.API.Services
{
	public interface IForgotPasswordAccountService
	{
		Task ForgotPasswordAccount(string email);
		Task<TokenDto?> ForgotPasswordConfirmationOtp(string token);
	}
}
