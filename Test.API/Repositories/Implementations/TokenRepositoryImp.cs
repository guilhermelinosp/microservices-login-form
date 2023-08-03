using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Test.API.Entities;

namespace Test.API.Repositories.Implementations
{
	public class TokenRepositoryImp : ITokenRepository
	{
		private readonly IMongoCollection<TokenEntity?> _collection; private readonly IConfiguration _configuration;

		public TokenRepositoryImp(IConfiguration configuration)
		{
			_configuration = configuration;
			var mongoClient = new MongoClient(configuration["MongoDb:ConnectionString"]);
			var mongoDatabase = mongoClient.GetDatabase(configuration["MongoDb:Database"]);
			_collection = mongoDatabase.GetCollection<TokenEntity>(configuration["MongoDb:Collection"])!;
		}

		private async Task AddToken(TokenEntity? token)
		{
			await _collection.InsertOneAsync(token);
		}

		public async Task<TokenEntity?> GetTokenByToken(string token)
		{
			return await _collection!.Find(Builders<TokenEntity>.Filter.Eq(x => x.Token, token)).FirstOrDefaultAsync();
		}

		public async Task<TokenEntity?> GetTokenByTokenOtp(string otp)
		{
			return await _collection!.Find(Builders<TokenEntity>.Filter.Eq(x => x.Otp, otp)).FirstOrDefaultAsync();
		}

		public async Task GenerateToken(string uuid, string type, string otp)
		{
			var claims = new List<Claim>
			{
				// Required claims
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
				new Claim(JwtRegisteredClaimNames.Sub, otp),
				new Claim(JwtRegisteredClaimNames.Typ, type),
				new Claim(JwtRegisteredClaimNames.Sid, uuid),
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Issuer = _configuration["JwtSecret:Issuer"],
				Audience = _configuration["JwtSecret:Audience"],
				Expires = DateTime.UtcNow.AddDays(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecret:Secret"]!)), SecurityAlgorithms.HmacSha256)
			};

			try
			{
				var token = tokenHandler.CreateToken(tokenDescriptor);
				var jwtHandler = tokenHandler.WriteToken(token);

				await AddToken(new TokenEntity
				{
					Token = jwtHandler,
					Otp = otp,
					Type = type,
					Uuid = uuid,
					ExpiresAt = DateTime.UtcNow.AddDays(1),
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				throw; // Rethrow the exception or handle as needed
			}
		}


		public async Task<TokenEntity?> ValidateTokenOtp(string otp, string type)
		{
			var token = await GetTokenByTokenOtp(otp);

			if (token == null) return null;

			if (token.ExpiresAt < DateTime.UtcNow) return null;

			if (token.Type != type) return null;

			return token.Otp != otp ? null : token;
		}

		public async Task<TokenEntity?> ValidateTokenForgotPassword(string tokenForgot, string type)
		{
			var token = await GetTokenByToken(tokenForgot);

			if (token == null) return null;

			if (token.ExpiresAt < DateTime.UtcNow) return null;

			return token.Type != type ? null : token;
		}

		public async Task<TokenPayload?> ValidateTokenAuthHeader(string token, string type)
		{
			var getTokenOtp = await GetTokenByToken(token);

			if (getTokenOtp == null) return null;

			if (getTokenOtp.ExpiresAt < DateTime.UtcNow) return null;

			return getTokenOtp.Type != type ? null : GetPayload(getTokenOtp.Token);
		}

		private TokenPayload? GetPayload(string token)
		{
			var handler = new JwtSecurityTokenHandler();

			var validationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = _configuration["JwtSecret:Issuer"]!,
				ValidAudience = _configuration["JwtSecret:Audience"]!,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecret:Secret"]!)),
			};

			handler.ValidateToken(token, validationParameters, out var validatedToken);
			var jwtToken = (JwtSecurityToken)validatedToken;
			return JsonConvert.DeserializeObject<TokenPayload>(jwtToken.Payload.SerializeToJson());
		}

		public async Task UpdateTokenOtp(string token, string otp)
		{
			var filter = Builders<TokenEntity>.Filter.Eq(x => x.Token, token);
			var update = Builders<TokenEntity>.Update.Set(x => x.Otp, otp);
			await _collection.UpdateOneAsync(filter!, update!);
		}
	}
}