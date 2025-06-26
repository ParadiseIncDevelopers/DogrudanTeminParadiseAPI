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
    public class AdminController : ControllerBase
    {
        private readonly ISuperAdminService _superAdminSvc;
        private readonly IAdminUserService _adminSvc;
        private readonly IUserService _userSvc;
        public AdminController(IAdminUserService adminSvc, IUserService userSvc, ISuperAdminService superAdminSvc) 
        {
            _superAdminSvc = superAdminSvc;
            _adminSvc = adminSvc;
            _userSvc = userSvc;
            
        }

        /// <summary>
        /// Hem yeni admin, hem de yeni normal user oluşturabilir.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            try
            {
                var created = await _adminSvc.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _adminSvc.GetByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        /// <summary>
        /// Tüm admin user'ları listeler.
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var admins = await _adminSvc.GetAllAsync();
            return Ok(admins);
        }

        [HttpGet("all-with-users")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AdminUserDto>>> GetAllWithUsers()
        {
            // 1) Çağıranın ID'sini al
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var currentUserId))
                return Unauthorized();

            // 2) Tüm adminleri çek, yalnızca kendisinin kaydını seç
            var allAdmins = await _adminSvc.GetAllAsync();
            var currentAdmin = allAdmins
                .Where(a => a.Id == currentUserId)
                .ToList();

            //İzinlerimizin olduğu kullanıcılara bakalım
            var list = await _superAdminSvc.GetAllAdminPermissionsAsync();
            var allPermissionUser = list[currentUserId];

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
            }).Where(x => allPermissionUser.Contains(x.Id));

            // 4) Birleştir ve döndür
            var result = currentAdmin
                .Concat(mappedUsers)
                .ToList();

            return Ok(result);
        }

        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] UpdateAdminPasswordDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            try
            {
                await _adminSvc.ChangePasswordAsync(userId, dto);
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
                await _adminSvc.AssignTitleAsync(id, titleId);
                return NoContent();
            }
            catch (Exception ex) when (ex is KeyNotFoundException)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAdminDto dto)
        {
            try
            {
                var updated = await _adminSvc.UpdateAsync(id, dto);
                if (updated == null)
                    return NotFound(new { error = "Kullanıcı bulunamadı." });

                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
