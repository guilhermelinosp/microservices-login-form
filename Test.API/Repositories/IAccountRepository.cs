using Test.API.Entities;

namespace Test.API.Repositories
{
	public interface IAccountRepository
	{
		Task<AccountEntity?> GetByEmail(string email);
		Task<AccountEntity?> GetByUuid(string uuid);
		Task<AccountEntity?> GetByPhone(string phone);
		Task<AccountEntity?> GetByUsername(string username);
		Task CreateAccount(AccountEntity account);
		Task ChangePassword(string uuid, string password);
		Task ChangePhone(string uuid, string phone);
		Task ChangeEmail(string uuid, string email);
		Task ChangeUsername(string uuid, string username);
		Task ActivateAccount(string uuid);
		Task DeactivateAccount(string uuid);
		Task Enable2Fa(string uuid);
		Task Disable2Fa(string uuid);
	}
}
