using ECommerce.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserClaimsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserClaimsController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("UpdateClaim")]
        public async Task<IActionResult> UpdateClaim(string username, string claimType, bool value)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            var existingClaims = await _userManager.GetClaimsAsync(user);
            var oldClaim = existingClaims.FirstOrDefault(c => c.Type == claimType);

            if (oldClaim != null)
                await _userManager.RemoveClaimAsync(user, oldClaim);

            await _userManager.AddClaimAsync(user, new Claim(claimType, value.ToString().ToLower()));

            return Ok(new { Message = $"Claim '{claimType}' updated to '{value}' for user '{username}'" });
        }

        [HttpGet("GetClaims")]
        public async Task<IActionResult> GetUserClaims(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            var claims = await _userManager.GetClaimsAsync(user);
            return Ok(claims.Select(c => new { c.Type, c.Value }));
        }
    }
}