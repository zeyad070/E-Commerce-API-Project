using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProductApp.BLL;
using ProductApp.Common.Utilities;

namespace ProductSystem.APIs.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountManager _accountManager;
        private readonly IValidator<RegisterDTO> _registerValidator;
        private readonly IValidator<LoginDTO> _loginValidator;

        public AuthController(
            IAccountManager accountManager,
            IValidator<RegisterDTO> registerValidator,
            IValidator<LoginDTO> loginValidator)
        {
            _accountManager = accountManager;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<string>>> Register([FromBody] RegisterDTO model)
        {
            var validation = await _registerValidator.ValidateAsync(model);
            if (!validation.IsValid)
                return BadRequest(ApiResponse<string>.BadRequest("Validation failed",
                    validation.Errors.Select(e => e.ErrorMessage).ToList()));

            var result = await _accountManager.RegisterAsync(model);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<string>.BadRequest(
                    "Registration failed",
                    result.Errors.Select(e => e.Description).ToList()));

            return Ok(ApiResponse<string>.Ok("User registered successfully"));
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Login([FromBody] LoginDTO model)
        {
            var validation = await _loginValidator.ValidateAsync(model);
            if (!validation.IsValid)
                return BadRequest(ApiResponse<AuthResponseDTO>.BadRequest("Validation failed",
                    validation.Errors.Select(e => e.ErrorMessage).ToList()));

            var response = await _accountManager.LoginAsync(model);

            if (response == null)
                return Unauthorized(ApiResponse<AuthResponseDTO>.Unauthorized("Invalid email or password"));

            return Ok(ApiResponse<AuthResponseDTO>.Ok(response, "Login successful"));
        }
    }
}
