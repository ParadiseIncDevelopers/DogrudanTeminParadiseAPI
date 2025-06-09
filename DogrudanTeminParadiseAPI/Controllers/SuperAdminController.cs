using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers.Attributes;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DogrudanTeminParadiseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuperAdminController : ControllerBase
    {
        private readonly ISuperAdminService _svc;
        public SuperAdminController(ISuperAdminService svc) => _svc = svc;

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] SuperAdminLoginDto dto)
        {
            var token = await _svc.AuthenticateAsync(dto);
            return Ok(new { token });
        }

        [HttpPut("make-active-or-passive-admin")]
        [Authorize(Roles = "SUPER_ADMIN")]
        public async Task<IActionResult> SetUserActive([FromBody] ChangeUserActiveStatusDto dto)
        {
            await _svc.SetUserActiveStatusAsync(dto);
            return NoContent();
        }

        [HttpPut("assign-user-permissions-to-admin")]
        [Authorize(Roles = "SUPER_ADMIN")]
        public async Task<IActionResult> AssignPermissions([FromBody] AssignUsersToAdminDto dto)
        {
            await _svc.AssignUsersToAdminAsync(dto);
            return NoContent();
        }

        [HttpGet("activities")]
        [Authorize(Roles = "SUPER_ADMIN")]
        public async Task<IActionResult> GetActivities()
        {
            var activities = await _svc.GetSystemActivityAsync();
            return Ok(activities);
        }
    }
}
