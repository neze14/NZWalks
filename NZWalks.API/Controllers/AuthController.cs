using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    // https://localhost:44372/api/v1/auth
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }

        //  POST: https://localhost:44372/api/v1/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username
            };

            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);

            if (identityResult.Succeeded)
            {
                // add roles to this user
                if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
                {
                    identityResult = await userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);

                    if (identityResult.Succeeded)
                    {
                        return Ok(new { message = "User registered successfully. Please log in." });
                    }
                }
            }

            return BadRequest(new { message = "User registration failed.", errors = identityResult.Errors });
        }

        //  POST: https://localhost:44372/api/v1/auth/login
        [HttpPost("login")]

        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDto.Username);
       
            if (user != null)
            {
                var isPasswordValid = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                if (isPasswordValid)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    if (roles != null)
                    {
                        // Generate JWT token here 
                        var jwtToken = tokenRepository.CreateJwtToken(user, roles.ToList());
                        
                        if (jwtToken == null)
                        {
                            return StatusCode(401, new { message = "Failed to generate access token." });
                        }

                        var response = new LoginResponseDto
                        {
                            message = "Login successful.",
                            accessToken = jwtToken
                        };

                        return Ok(response);
                    }
                }
            }

            return BadRequest(new { message = "Invalid username or password." });
        }
    }
}
