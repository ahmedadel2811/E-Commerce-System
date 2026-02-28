using ECommerce.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("AddRoleToUser")]
        public async Task<IActionResult> AddRoleToUser(string username, string role)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            if (!await _roleManager.RoleExistsAsync(role))
                return BadRequest(new { Message = "Role does not exist" });

            var result = await _userManager.AddToRoleAsync(user, role);

            if (result.Succeeded)
                return Ok(new { Message = $"Role '{role}' added to user '{username}'" });

            return BadRequest(result.Errors);
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(string role)
        {
            if (await _roleManager.RoleExistsAsync(role))
                return BadRequest(new { Message = "Role already exists" });

            var result = await _roleManager.CreateAsync(new IdentityRole(role));

            if (result.Succeeded)
                return Ok(new { Message = $"Role '{role}' created successfully" });

            return BadRequest(result.Errors);
        }
    }
}
