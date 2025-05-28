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
        public async Task<IActionResult> GetAllWithUsers()
        {
            // 1) Çağıranın ID'sini al
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var currentUserId))
                return Unauthorized();

            // 2) Tüm normal kullanıcıları çek
            var normalUsers = await _userSvc.GetAllAsync();

            // 3) Tüm adminleri çek, ama sadece kendisinin kaydını al
            var allAdmins = await _svc.GetAllAsync();
            var currentAdmin = allAdmins.FirstOrDefault(a => a.Id == currentUserId);

            // 4) Sonucu dön
            return Ok(new
            {
                Admins = currentAdmin != null
                            ? new[] { currentAdmin }
                            : Array.Empty<AdminUserDto>(),
                Users = normalUsers
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _svc.AuthenticateAsync(dto);
                return Ok(new { token });
            }
            catch
            {
                return Unauthorized("TC veya parola hatalı");
            }
        }
    }
}
