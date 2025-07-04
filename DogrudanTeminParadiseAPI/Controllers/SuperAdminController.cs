using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Dto.Logger;
using DogrudanTeminParadiseAPI.Helpers.Attributes;
using DogrudanTeminParadiseAPI.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DogrudanTeminParadiseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [CallLogs]
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

        [HttpGet("active-passives")]
        [Authorize(Roles = "SUPER_ADMIN")]
        public async Task<IActionResult> GetActivePassiveUsers()
        {
            var map = await _svc.GetActivePassiveUsersAsync();
            return Ok(map);
        }

        [HttpGet("permissions")]
        [Authorize(Roles = "SUPER_ADMIN")]
        public async Task<IActionResult> GetAllAdminPermissions()
        => Ok(await _svc.GetAllAdminPermissionsAsync());

        [HttpGet("permissions/{adminId}")]
        [Authorize(Roles = "SUPER_ADMIN")]
        public async Task<IActionResult> GetAdminPermissions(Guid adminId)
        {
            try
            {
                var list = await _svc.GetAdminPermissionsAsync(adminId);
                return Ok(list);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("reset-password")]
        [Authorize(Roles = "SUPER_ADMIN")]
        public async Task<IActionResult> ResetPassword([FromBody] UpdateForgotPasswordDto dto)
        {
            await _svc.ResetPasswordAsync(dto);
            return NoContent();
        }

        [HttpGet("page-activities")]
        public async Task<IActionResult> GetPageActivities([FromQuery] PageQueryParameters q)
        {
            try
            {
                var activities = await _svc.GetPageActivitiesAsync(q);
                return Ok(activities);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, new { error = $"Logger API hatası: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
