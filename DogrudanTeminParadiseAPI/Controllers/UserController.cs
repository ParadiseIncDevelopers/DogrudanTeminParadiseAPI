using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers.Attributes;
using DogrudanTeminParadiseAPI.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DogrudanTeminParadiseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [CallLogs]
    public class UserController : ControllerBase
    {
        private readonly IUserService _svc;

        public UserController(IUserService svc)
        {
            _svc = svc;
        }

        [HttpPost("{adminId}")]
        public async Task<IActionResult> Create(string adminId, [FromBody] CreateUserDto dto)
        {
            try
            {
                var created = await _svc.CreateAsync(dto, adminId);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var users = await _svc.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var callerRole = User.FindFirstValue(ClaimTypes.Role);
            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (callerRole == "User" && callerId != id.ToString())
                return Forbid();

            var user = await _svc.GetByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            var updated = await _svc.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _svc.DeleteAsync(id);
            return NoContent();
        }

        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] UpdateUserPasswordDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            try
            {
                await _svc.ChangePasswordAsync(userId, dto);
                return NoContent();
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is InvalidOperationException || ex is KeyNotFoundException)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}/assign-title/{titleId}")]
        [Authorize]
        public async Task<IActionResult> AssignTitle(Guid id, Guid titleId)
        {
            try
            {
                await _svc.AssignTitleAsync(id, titleId);
                return NoContent();
            }
            catch (Exception ex) when (ex is KeyNotFoundException)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}
