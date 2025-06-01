using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Service.Abstract;
using DogrudanTeminParadiseAPI.Service.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DogrudanTeminParadiseAPI.Controllers
{
    // Controllers/AdminController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminUserService _svc;
        private readonly IUserService _userSvc;
        public AdminController(IAdminUserService svc, IUserService userSvc) 
        {
            _userSvc = userSvc;
            _svc = svc;
        }

        /// <summary>
        /// Hem yeni admin, hem de yeni normal user oluşturabilir.
        /// </summary>
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

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _svc.GetByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        /// <summary>
        /// Tüm admin user'ları listeler.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var admins = await _svc.GetAllAsync();
            return Ok(admins);
        }

        [HttpGet("all-with-users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<AdminUserDto>>> GetAllWithUsers()
        {
            // 1) Çağıranın ID'sini al
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var currentUserId))
                return Unauthorized();

            // 2) Tüm adminleri çek, yalnızca kendisinin kaydını seç
            var allAdmins = await _svc.GetAllAsync();
            var currentAdmin = allAdmins
                .Where(a => a.Id == currentUserId)
                .ToList();

            // 3) Tüm normal kullanıcıları çek ve AdminUserDto’ya dönüştür
            var normalUsers = await _userSvc.GetAllAsync();
            var mappedUsers = normalUsers.Select(u => new AdminUserDto
            {
                Id = u.Id,
                Name = u.Name,
                Surname = u.Surname,
                Email = u.Email,
                Tcid = u.Tcid,
                UserType = u.UserType,
                TitleId = u.TitleId,
                Permissions = u.Permissions,
                PublicInstitutionName = null
            });

            // 4) Birleştir ve döndür
            var result = currentAdmin
                .Concat(mappedUsers)
                .ToList();

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _svc.AuthenticateAsync(dto);
                return Ok(new { token });
            }
            catch(Exception ex)
            {
                return Unauthorized("TC veya parola hatalı");
            }
        }

        [HttpPut("change-password")]
        [Authorize(Roles = "Admin")]
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

        [HttpPut("{id}/assign-title/{titleId}")]
        [Authorize(Roles = "Admin")]
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
