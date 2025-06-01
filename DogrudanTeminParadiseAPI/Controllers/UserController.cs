using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers.Attributes;
using DogrudanTeminParadiseAPI.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DogrudanTeminParadiseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [CallLogs]
    public class UserController : ControllerBase
    {
        private readonly IUserService _svc;
        private readonly IConfiguration _config;

        public UserController(IUserService svc, IConfiguration config)
        {
            _svc = svc;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            try
            {
                var created = await _svc.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _svc.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var callerRole = User.FindFirstValue(ClaimTypes.Role);
            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (callerRole == "User" && callerId != id.ToString())
                return Forbid();

            var user = await _svc.GetByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            var updated = await _svc.UpdateAsync(id, dto);
            return updated == null
                ? NotFound()
                : Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _svc.DeleteAsync(id);
            return NoContent();
        }

        // LOGIN -- TC ve şifre ile giriş
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _svc.AuthenticateAsync(dto);
                return Ok(new { token });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "TC veya şifre hatalı." });
            }
        }

        [HttpPut("{id}/title/{titleId}")]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> AssignTitle(Guid id, Guid titleId)
        {
            try
            {
                var updated = await _svc.AssignTitleAsync(id, titleId);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}
