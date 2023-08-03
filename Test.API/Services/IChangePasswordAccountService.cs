using Test.API.Dtos.Account;

namespace Test.API.Services
{
	public interface IChangePasswordAccountService
	{
		Task ChangePasswordAccount(ChangePasswordAccountDto changePasswordAccountDTO, string token);
	}
}
