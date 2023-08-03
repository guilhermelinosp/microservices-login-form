using Microsoft.EntityFrameworkCore;
using Test.API.Entities;

namespace Test.API.Repositories.Context
{
	public class DatabaseContext : DbContext
	{
		public DatabaseContext() { }
		public DatabaseContext(DbContextOptions<DatabaseContext> opt) : base(opt) { }
		public DbSet<AccountEntity>? Accounts { get; set; }
	}
}
