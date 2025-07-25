using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers.Attributes;
using DogrudanTeminParadiseAPI.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DogrudanTeminParadiseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [CallLogs]
    public class UserAvatarController : ControllerBase
    {
        private readonly IUserAvatarService _svc;
        public UserAvatarController(IUserAvatarService svc) => _svc = svc;

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _svc.GetAllAsync());
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserAvatarDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByUserOrAdminId), new { userOrAdminId = created.UserOrAdminId }, created);
        }

        [HttpGet("{userOrAdminId}")]
        public async Task<IActionResult> GetByUserOrAdminId(Guid userOrAdminId)
        {
            var dto = await _svc.GetByUserOrAdminIdAsync(userOrAdminId);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpPut("{userOrAdminId}")]
        public async Task<IActionResult> Update(Guid userOrAdminId, [FromBody] UpdateUserAvatarDto dto)
        {
            var updated = await _svc.UpdateAsync(userOrAdminId, dto);
            return updated == null ? NotFound() : Ok(updated);
        }
    }
}
