using Test.API.Dtos.Profile;

namespace Test.API.Services
{
	public interface IChangePasswordProfileService
	{
		Task ChangePasswordProfile(ChangePasswordProfileDto changePasswordProfileDto, string token);
	}
}
