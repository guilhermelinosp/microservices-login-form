using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Test.API.Dtos.Profile;
using Test.API.Exceptions;
using Test.API.Services;

namespace Test.API.Controllers
{
	[ApiController]
	[Route("api/private/profile/")]
	public class ProfileController : ControllerBase
	{
		private readonly IChangePasswordProfileService _changePasswordProfile;
		private readonly IChangeUsernameProfileService _changeUsernameProfile;
		private readonly IEnable2FAProfileServices _enable2FaProfile;
		private readonly IDisable2FaProfileService _disable2FaProfile;
		private readonly IDeactivateAccount _deactivateAccount;

		public ProfileController(
				IChangePasswordProfileService changePasswordProfile,
			   IEnable2FAProfileServices enable2FaProfile, IDisable2FaProfileService disable2FaProfile, IDeactivateAccount deactivateAccount, IChangeUsernameProfileService changeUsernameProfile)
		{
			_changePasswordProfile = changePasswordProfile;
			_enable2FaProfile = enable2FaProfile;
			_disable2FaProfile = disable2FaProfile;
			_deactivateAccount = deactivateAccount;
			_changeUsernameProfile = changeUsernameProfile;
		}

		[Authorize(AuthenticationSchemes = "Bearer")]
		[HttpPost("changepassword")]
		public async Task<IActionResult> ChangePasswordProfile([FromBody] ChangePasswordProfileDto changePasswordModel)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			try
			{
				await _changePasswordProfile.ChangePasswordProfile(changePasswordModel, Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));

				return Ok(new { Message = "Password changed successfully." });
			}
			catch (TokenOtpInvalidException)
			{
				return BadRequest(new { Error = "Invalid or expired activation token. Please check your OTP." });
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

		[Authorize(AuthenticationSchemes = "Bearer")]
		[HttpPost("changeusername")]
		public async Task<IActionResult> ChangeUsernameProfile([FromBody] ChangeUsernameProfileDto changeUsername)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				await _changeUsernameProfile.ChangeUsernameProfile(changeUsername, Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));

				return Ok(new { Message = "Username changed successfully." });
			}
			catch (TokenOtpInvalidException)
			{
				return BadRequest(new { Error = "Invalid or expired activation token. Please check your OTP." });
			}
			catch (AccountUsernameAlreadyExistsException)
			{
				return BadRequest(new { Error = "Username is already being used. Please check your username." });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An error occurred during sign-in. Please try again later." });
			}
		}

		[Authorize(AuthenticationSchemes = "Bearer")]
		[HttpPost("enable2fa")]
		public async Task<IActionResult> Enable2FaProfile([FromBody] Enable2FaProfileDto phone)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				await _enable2FaProfile.Enable2FAProfile(phone.Phone, Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));

				return Ok(new { Message = "Confirm your OTP Token. Check your phone number." });
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

		[Authorize(AuthenticationSchemes = "Bearer")]
		[HttpGet("enable2fa/{otp}")]
		public async Task<IActionResult> Enable2FaConfirmationOtp([FromRoute] string otp)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				await _enable2FaProfile.Enable2FAConfirmationOTP(otp, Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));

				return Ok(new { Message = "Two-Factor Authentication has been enabled successfully." });
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

		[Authorize(AuthenticationSchemes = "Bearer")]
		[HttpPost("disable2fa")]
		public async Task<IActionResult> DisableTwoFactorAuthentication()
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				await _disable2FaProfile.Disable2FaProfile(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));

				return Ok(new { Message = "Confirm your OTP Token. Check your phone number." });
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

		[Authorize(AuthenticationSchemes = "Bearer")]
		[HttpGet("disable2fa/{otp}")]
		public async Task<IActionResult> DisableTwoFactorAuthentication([FromRoute] string otp)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				await _disable2FaProfile.Disable2FaConfirmationOtp(otp, Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));

				return Ok(new { Message = "Two-Factor Authentication has been disabled successfully." });

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

		[Authorize(AuthenticationSchemes = "Bearer")]
		[HttpPost("deactivateaccount")]
		public async Task<IActionResult> DeactivateAccount()
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				await _deactivateAccount.DeactivateAccount(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));

				return Ok(new { Message = "Confirm your OTP Token. Check your phone number." });
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

		[Authorize(AuthenticationSchemes = "Bearer")]
		[HttpGet("deactivateaccount/{otp}")]
		public async Task<IActionResult> DeactivateAccountConfirmationOtp([FromRoute] string otp)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				await _deactivateAccount.DeactivateAccountConfirmationOtp(otp, Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));

				return Ok(new { Message = "Your account has been successfully deactivated." });
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
	}
}