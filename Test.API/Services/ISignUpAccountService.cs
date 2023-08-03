using Test.API.Dtos.Account;

namespace Test.API.Services
{
	public interface ISignUpAccountService
	{
		Task SignUpAccount(SignUpAccountDto? signUpAccountDTO);
		Task SignUpAccountActivation(string otp);
	}
}
