using ECommerce.Core.DTOs;
using ECommerce.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Commerce_System.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _config;


        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(ILogger<AccountController> logger, UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _logger = logger;
            _userManager = userManager;
            _config = config;
        }

        [HttpPost("Register")]

        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new
                    {
                        Message = "Validation failed",
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });



                // Check if username exists
                var existingUser = await _userManager.FindByNameAsync(dto.UserName);
                if (existingUser != null)
                {
                    return BadRequest(new { message = $"Username '{dto.UserName}' is already taken." });
                }

                // Check if email exists
                var existingEmail = await _userManager.FindByEmailAsync(dto.Email);
                if (existingEmail != null)
                {
                    return BadRequest(new { message = $"The email '{dto.Email}' is already associated with an account." });
                }

                var user = new ApplicationUser
                {
                    UserName = dto.UserName,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    Address = dto.Address,
                    CreatedAt = DateTime.UtcNow
                };


                var result = await _userManager.CreateAsync(user, dto.Password);


                if (result.Succeeded)
                {

                    // Assign default role if needed
                    await _userManager.AddToRoleAsync(user, "Admin");

                    //  Add Claims for permissions
                    await _userManager.AddClaimAsync(user, new Claim("CanAdd", "false"));
                    await _userManager.AddClaimAsync(user, new Claim("CanEdit", "false"));
                    await _userManager.AddClaimAsync(user, new Claim("CanDelete", "false"));
                    await _userManager.AddClaimAsync(user, new Claim("CanView", "false"));

                    _logger.LogInformation("User {UserName} registered successfully", dto.UserName);
                    return Ok(new RegisterResponse
                    {
                        Success = true,
                        Message = "Registration successful!"
                    });
                }

                var errors = result.Errors.Select(e => e.Description);
                _logger.LogWarning("Registration failed for {UserName}: {Errors}", dto.UserName, errors);

                return BadRequest(new RegisterResponse
                {
                    Success = false,
                    Message = "Registration failed",
                    Errors = errors
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for {UserName}", dto.UserName);
                return StatusCode(500, new { Message = "An error occurred during registration" });
            }
        }

        [HttpPost("Login")]

        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new
                    {
                        Message = "Validation failed",
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });


                var user = await _userManager.FindByNameAsync(dto.UserName);
                if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                {
                    _logger.LogWarning("Invalid login attempt for username: {UserName}", dto.UserName);
                    return BadRequest(new { Message = "Invalid username or password" });
                }



                //if (user == null)
                //{
                //    _logger.LogWarning("Failed login attempt for username: {UserName}", dto.UserName);
                //    return BadRequest(new { Message = "Invalid username or password" });
                //}




                // Update last login
                user.LastLogin = DateTime.UtcNow;

                await _userManager.UpdateAsync(user);


                // Get user claims + roles
                var claims = await GetUserClaims(user);

                // Generate token that holds all info
                var token = GenerateJwtToken(claims);
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                _logger.LogInformation("User {UserName} logged in successfully", user.UserName);

                //  Return only the token (and optionally expiration)
                return Ok(new
                {
                    token = tokenString
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for {UserName}", dto.UserName);
                return StatusCode(500, new { Message = "An error occurred during login" });
            }
        }


        private async Task<List<Claim>> GetUserClaims(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FullName", user.FullName),
                new Claim("Address", user.Address)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


            //  Add user specific claims (permissions)
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            return claims;
        }

        private JwtSecurityToken GenerateJwtToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(Convert.ToDouble(_config["Jwt:ExpirationHours"] ?? "1")),
                signingCredentials: creds
            );
        }

    }
}