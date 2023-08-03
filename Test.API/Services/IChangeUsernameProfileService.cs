using Test.API.Dtos.Profile;

namespace Test.API.Services
{
	public interface IChangeUsernameProfileService
	{
		Task ChangeUsernameProfile(ChangeUsernameProfileDto username, string token);
	}
}
