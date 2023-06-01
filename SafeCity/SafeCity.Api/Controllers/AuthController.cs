using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SafeCity.Api.Entity;
using SafeCity.Api.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SafeCity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpPost]
        [Route("Login/CreateAccount")]
        public async Task<IActionResult> SendOtpOnPhone(CreateAccountRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                user = new AppUser { UserName = request.UserName, FullName = request.FullName };
                var createUserResult = await _userManager.CreateAsync(user, request.Password);
                if (!createUserResult.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create user");
                }
                await _userManager.AddToRoleAsync(user, UserRoles.Client);
            }
            return Ok();
        }

        [HttpPost]
        [Route("Login/Login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponse))]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "User not found");
            }

            bool isValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);

            if (isValidPassword)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                JwtSecurityToken token = await GetToken(user);
                return Ok(new TokenResponse
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo,
                });
            }
            else
            {
                return BadRequest("Invalid passsword");
            }
        }


        private async Task<JwtSecurityToken> GetToken(AppUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SafeCityContextSafeCityContext"));

            var token = new JwtSecurityToken(
                issuer: "SafeCityContextSafeCityContext",
                audience: "SafeCityContextSafeCityContext",
                expires: DateTime.Now.AddHours(720),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

    }

    public class CreateAccountRequest: LoginRequest
    {

        public string FullName { get; set; }
    }

    public class LoginRequest 
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class TokenResponse
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
