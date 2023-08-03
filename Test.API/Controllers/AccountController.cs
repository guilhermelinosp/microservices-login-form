using Microsoft.AspNetCore.Mvc;
using Test.API.Dtos.Account;
using Test.API.Exceptions;
using Test.API.Services;

namespace Test.API.Controllers
{
	[ApiController]
	[Route("api/public/account/")]
	public class AccountController : ControllerBase
	{
		private readonly ISignUpAccountService _signUpAccount;
		private readonly ISignInAccountService _signInAccount;
		private readonly IForgotPasswordAccountService _forgotPasswordAccount;
		private readonly IChangePasswordAccountService _changePasswordAccount;

		public AccountController(
				ISignUpAccountService signUpAccount,
				ISignInAccountService signInAccount,
				IForgotPasswordAccountService forgotPasswordAccount,
				IChangePasswordAccountService changePasswordAccount)
		{
			_signUpAccount = signUpAccount;
			_signInAccount = signInAccount;
			_forgotPasswordAccount = forgotPasswordAccount;
			_changePasswordAccount = changePasswordAccount;
		}

		[HttpPost]
		[Route("signin")]
		public async Task<IActionResult> SignInAccount([FromBody] SignInAccountDto account)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				await _signInAccount.SignInAccount(account);
				return Unauthorized(new { Message = "Confirm your login with OTP Token. Check your email or phone number." });
			}
			catch (AccountNotFoundException)
			{
				return Unauthorized(new { Error = "Account Not Found. Please check your email." });
			}
			catch (AccountNotActivatedException)
			{
				return Unauthorized(new { Error = "Account not activated. Please check your email for the activation link." });
			}
			catch (AccountInvalidCredentialsException)
			{
				return Unauthorized(new { Error = "Invalid password. Please check your credentials." });
			}
			catch (AccountBlockedException)
			{
				return Unauthorized(new { Error = "Account has been deactivated." });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An error occurred during sign-in. Please try again later." });
			}
		}

		[HttpGet]
		[Route("signin/{otp}")]
		public async Task<IActionResult> SignInAccountConfirmationOtp([FromRoute] string otp)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				var tokendto = await _signInAccount.SignInAccountConfirmationOtp(otp);
				return Ok(tokendto);
			}
			catch (TokenOtpInvalidException)
			{
				return BadRequest(new { Error = "Invalid or expired activation token. Please check your OTP." });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An error occurred during sign-in. Please try again later." });
			}
		}

		[HttpPost]
		[Route("signup")]
		public async Task<IActionResult> SignUpAccount([FromBody] SignUpAccountDto account)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				await _signUpAccount.SignUpAccount(account);
				return Ok(new { Message = "Confirm your registered account, we will send you a confirmation email. Team Test API" });
			}
			catch (AccountNotFoundException)
			{
				return BadRequest(new { Error = "E-mail is already being used. Please check your email. Please check your email." });
			}
			catch (AccountEmailInvalidException)
			{
				return BadRequest(new { Error = "Invalid email format. Please check your email." });
			}
			catch (AccountInvalidCredentialsException)
			{
				return BadRequest(new { Error = "Invalid password, passwords do not match or the password must be less than 8 characters long.. Please check your credentials." });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An error occurred during sign-in. Please try again later." });
			}
		}

		[HttpGet]
		[Route("signup/{otp}")]
		public async Task<IActionResult> SignUpAccountActivation([FromRoute] string otp)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				await _signUpAccount.SignUpAccountActivation(otp);
				return Ok(new { Message = "Account activated successfully!. Team Test API" });
			}
			catch (TokenOtpInvalidException)
			{
				return BadRequest(new { Error = "Invalid or expired activation token. Please request a new one." });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An error occurred during sign-in. Please try again later." });
			}
		}

		[HttpPost]
		[Route("forgotpassword")]
		public async Task<IActionResult?> ForgotPasswordAccount([FromBody] ForgotPasswordAccountDto email)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			try
			{
				await _forgotPasswordAccount.ForgotPasswordAccount(email.Email);
				return Ok(new { Message = "Confirm your forgotten password request with OTP Token. Please verify your email or phone number." });
			}
			catch (AccountNotFoundException)
			{
				return Unauthorized(new { Error = "Account Not Found. Please check your email." });
			}
		}

		[HttpGet]
		[Route("forgotpassword/{otp}")]
		public async Task<IActionResult?> ForgotPasswordConfirmationOtp([FromRoute] string otp)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			try
			{
				var tokendto = await _forgotPasswordAccount.ForgotPasswordConfirmationOtp(otp);
				return Ok(tokendto);
			}
			catch (TokenOtpInvalidException)
			{
				return BadRequest(new { Error = "Invalid or expired activation token. Please request a new one." });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An error occurred during sign-in. Please try again later." });
			}
		}

		[HttpPost]
		[Route("changepassword/{token}")]
		public async Task<IActionResult?> ChangePasswordAccount([FromBody] ChangePasswordAccountDto changePasswordModel, [FromRoute] string token)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				await _changePasswordAccount.ChangePasswordAccount(changePasswordModel, token);

				return Ok(new { Message = "Password changed successfully." });
			}
			catch (TokenOtpInvalidException)
			{
				return BadRequest(new { Error = "Invalid or expired activation token. Please request a new one." });
			}
			catch (AccountInvalidCredentialsException)
			{
				return BadRequest(new { Error = "Invalid password, passwords do not match or the password must be less than 8 characters long.. Please check your credentials." });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An error occurred during sign-in. Please try again later." });
			}
		}
	}
}
