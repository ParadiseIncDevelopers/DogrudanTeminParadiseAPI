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
    [CallLogs]
    public class SuperAdminController : ControllerBase
    {
        private readonly ISuperAdminService _svc;

        public SuperAdminController(ISuperAdminService svc)
        {
            _svc = svc;
        }

        /// <summary>
        /// Yeni bir SuperAdmin oluşturur. (DTO içinde UserType belirtmeye gerek yok, sabit olarak "SUPER_ADMIN" atanacaktır.)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSuperAdminDto dto)
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

        /// <summary>
        /// ID ile tek bir SuperAdmin'i döner.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "SUPER_ADMIN")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var admin = await _svc.GetByIdAsync(id);
            return admin == null ? NotFound() : Ok(admin);
        }

        /// <summary>
        /// Tüm SuperAdmin'leri listeler.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SUPER_ADMIN")]
        public async Task<IActionResult> GetAll()
        {
            var allAdmins = await _svc.GetAllAsync();
            return Ok(allAdmins);
        }

        /// <summary>
        /// SuperAdmin login işlemi. Başarılıysa JWT token döner.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _svc.AuthenticateAsync(dto);
                return Ok(new { token });
            }
            catch (Exception)
            {
                return Unauthorized("TC veya parola hatalı");
            }
        }

        /// <summary>
        /// Şu anki SuperAdmin, parola değiştirmek isterse kullanılır.
        /// </summary>
        [HttpPut("change-password")]
        [Authorize(Roles = "SUPER_ADMIN")]
        public async Task<IActionResult> ChangePassword([FromBody] UpdateAdminPasswordDto dto)
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

        /// <summary>
        /// Mevcut SuperAdmin bilgilerini (Name, Surname, Email, Tcid) güncellemek için.
        /// </summary>
        [HttpPut("update-info")]
        [Authorize(Roles = "SUPER_ADMIN")]
        public async Task<IActionResult> UpdateInfo([FromBody] UpdateSuperAdminDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            try
            {
                await _svc.UpdateAsync(userId, dto);
                return NoContent();
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is KeyNotFoundException)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
