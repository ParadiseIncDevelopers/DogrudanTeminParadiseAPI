using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Service.Abstract;
using DogrudanTeminParadiseAPI.Service.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DogrudanTeminParadiseAPI.Controllers
{
    // Controllers/AdminController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminUserService _svc;
        public AdminController(IAdminUserService svc) => _svc = svc;

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
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _svc.GetByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        /// <summary>
        /// Tüm admin user'ları listeler.
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var admins = await _svc.GetAllAsync();
            return Ok(admins);
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
