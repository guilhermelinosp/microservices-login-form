using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Test.API.Entities;
using Test.API.Repositories.Context;

namespace Test.API.Repositories.Implementations
{
	public class AccountRepositoryImp : IAccountRepository
	{
		private readonly DatabaseContext _context;
		private readonly IConfiguration _configuration;
		public AccountRepositoryImp(DatabaseContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}

		public async Task<AccountEntity?> GetByEmail(string email)
		{
			return await _context.Accounts!.FirstOrDefaultAsync(p => p.Email == email);
		}

		public async Task<AccountEntity?> GetByUuid(string uuid)
		{
			return await _context.Accounts!.FirstOrDefaultAsync(p => p.Uuid == uuid);
		}

		public async Task<AccountEntity?> GetByPhone(string phone)
		{
			return await _context.Accounts!.FirstOrDefaultAsync(p => p.Phone == phone);
		}

		public async Task<AccountEntity?> GetByUsername(string username)
		{
			return await _context.Accounts!.FirstOrDefaultAsync(p => p.Username == username);
		}

		public async Task CreateAccount(AccountEntity account)
		{
			var parameters = new
			{
				uuid = account.Uuid,
				username = account.Username,
				email = account.Email,
				password = account.Password
			};

			await using var sqlConnection = new SqlConnection(_configuration["SqlServer:ConnectionString"]);
			await sqlConnection.OpenAsync();
			await sqlConnection.ExecuteAsync("SP_INSERT_ACCOUNT", parameters, commandType: CommandType.StoredProcedure);
		}

		public async Task ChangePassword(string uuid, string password)
		{
			await using var sqlConnection = new SqlConnection(_configuration["SqlServer:ConnectionString"]);
			await sqlConnection.OpenAsync();
			await sqlConnection.QueryAsync($"UPDATE [TB_ACCOUNT] SET [Password] = '{password}', [UpdatedAt] = GETDATE() WHERE [UUID] = '{uuid}'");
		}

		public async Task ChangeEmail(string uuid, string email)
		{
			await using var sqlConnection = new SqlConnection(_configuration["SqlServer:ConnectionString"]);
			await sqlConnection.OpenAsync();
			await sqlConnection.QueryAsync($"UPDATE [TB_ACCOUNT] SET [Email] = '{email}', [UpdatedAt] = GETDATE() WHERE [UUID] = '{uuid}'");
		}

		public async Task ChangePhone(string uuid, string phone)
		{
			await using var sqlConnection = new SqlConnection(_configuration["SqlServer:ConnectionString"]);
			await sqlConnection.OpenAsync();
			await sqlConnection.QueryAsync($"UPDATE [TB_ACCOUNT] SET [Phone] = '{phone}', [UpdatedAt] = GETDATE() WHERE [UUID] = '{uuid}'");
		}

		public async Task ChangeUsername(string uuid, string username)
		{
			await using var sqlConnection = new SqlConnection(_configuration["SqlServer:ConnectionString"]);
			await sqlConnection.OpenAsync();
			await sqlConnection.QueryAsync($"UPDATE [TB_ACCOUNT] SET [Username] = '{username}', [UpdatedAt] = GETDATE() WHERE [UUID] = '{uuid}'");
		}

		public async Task ActivateAccount(string uuid)
		{
			await using var sqlConnection = new SqlConnection(_configuration["SqlServer:ConnectionString"]);
			await sqlConnection.OpenAsync();
			await sqlConnection.QueryAsync($"UPDATE [TB_ACCOUNT] SET [ActiveAccount] = 1, [UpdatedAt] = GETDATE() WHERE [UUID] = '{uuid}'");
		}

		public async Task DeactivateAccount(string uuid)
		{
			await using var sqlConnection = new SqlConnection(_configuration["SqlServer:ConnectionString"]);
			await sqlConnection.OpenAsync();
			await sqlConnection.QueryAsync($"UPDATE [TB_ACCOUNT] SET [ActiveAccount] = 0, [Blocked] = 1, [TwoFactorAuth] = 0, [UpdatedAt] = GETDATE() WHERE [UUID] = '{uuid}'");
		}

		public async Task Enable2Fa(string uuid)
		{
			await using var sqlConnection = new SqlConnection(_configuration["SqlServer:ConnectionString"]);
			await sqlConnection.OpenAsync();
			await sqlConnection.QueryAsync($"UPDATE [TB_ACCOUNT] SET [TwoFactorAuth] = 1, [UpdatedAt] = GETDATE() WHERE [UUID] = '{uuid}'");
		}

		public async Task Disable2Fa(string uuid)
		{
			await using var sqlConnection = new SqlConnection(_configuration["SqlServer:ConnectionString"]);
			await sqlConnection.OpenAsync();
			await sqlConnection.QueryAsync($"UPDATE [TB_ACCOUNT] SET [TwoFactorAuth] = 0, [Phone] = null, [UpdatedAt] = GETDATE() WHERE [UUID] = '{uuid}'");
		}
	}
}
