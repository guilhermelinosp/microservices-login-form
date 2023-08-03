using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Test.API.ExternalServices;
using Test.API.ExternalServices.Implementations;
using Test.API.Repositories;
using Test.API.Repositories.Context;
using Test.API.Repositories.Implementations;
using Test.API.Services;
using Test.API.Services.Implementations.Account;
using Test.API.Services.Implementations.Profile;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Swagger configuration
builder.Services.AddSwaggerGen(opt =>
{
	opt.SwaggerDoc("V1", new OpenApiInfo
	{
		Title = "Test API Swagger Documentation",
		Version = "V1",
	});

	opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = @"Enter 'Bearer' [space] and your token!",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	});

	opt.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				},
				Scheme = "Bearer",
				Name = "Bearer",
				In= ParameterLocation.Header
			},
			new List<string>()
		}
	});
});

// Add CORS configuration
builder.Services.AddCors(opt => opt.AddDefaultPolicy(cors =>
{
	cors.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

// Add authentication with JWT Bearer
var configuration = builder.Configuration;
builder.Services.AddAuthentication("Bearer").AddJwtBearer(opt =>
{
	opt.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = configuration["JwtSecret:Issuer"],
		ValidAudience = configuration["JwtSecret:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSecret:Secret"]!)),
	};
});

// Add authorization
builder.Services.AddAuthorization();

// Add DbContext with SQL Server
builder.Services.AddDbContext<DatabaseContext>(opt => opt.UseSqlServer(configuration["SqlServer:ConnectionString"]));

// Register repositories
builder.Services.AddScoped<ITokenRepository, TokenRepositoryImp>();
builder.Services.AddScoped<IAccountRepository, AccountRepositoryImp>();

// Register services
builder.Services.AddScoped<ISignUpAccountService, SignUpAccountServiceImp>();
builder.Services.AddScoped<ISignInAccountService, SignInAccountServiceImp>();
builder.Services.AddScoped<IForgotPasswordAccountService, ForgotPasswordAccountServiceImp>();
builder.Services.AddScoped<IChangePasswordAccountService, ChangePasswordAccountServiceImp>();
builder.Services.AddScoped<IChangePasswordProfileService, ChangePasswordProfileServiceImp>();
builder.Services.AddScoped<IChangeUsernameProfileService, ChangeUsernameProfileServiceImp>();
builder.Services.AddScoped<IDeactivateAccount, DeactivateAccountImp>();
builder.Services.AddScoped<IEnable2FAProfileServices, Enable2FaProfileServiceImp>();
builder.Services.AddScoped<IDisable2FaProfileService, Disable2FaProfileServiceImp>();

// Register external service implementations
builder.Services.AddScoped<ITwilio, TwilioImp>();
builder.Services.AddScoped<ISendGrid, SendGridImp>();

// Add Application Insights telemetry
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(opt =>
	{
		opt.SwaggerEndpoint("/swagger/V1/swagger.json", "Test API V1");
	});
}

app.MapControllers();
app.Run();
