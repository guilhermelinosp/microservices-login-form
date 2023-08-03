using Test.API.Entities;

namespace Test.API.Repositories
{
	public interface ITokenRepository
	{
		Task<TokenEntity?> GetTokenByToken(string token);
		Task<TokenEntity?> GetTokenByTokenOtp(string otp);
		Task GenerateToken(string uuid, string type, string otp);
		Task<TokenEntity?> ValidateTokenOtp(string otp, string type);
		Task<TokenEntity?> ValidateTokenForgotPassword(string tokenForgot, string type);
		Task<TokenPayload?> ValidateTokenAuthHeader(string token, string type);
		Task UpdateTokenOtp(string token, string otp);
	}
}
