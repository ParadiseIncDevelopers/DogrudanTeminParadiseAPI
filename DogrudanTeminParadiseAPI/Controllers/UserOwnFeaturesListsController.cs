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
    public class UserOwnFeaturesListsController : ControllerBase
    {
        private readonly IUserOwnFeaturesListService _svc;

        public UserOwnFeaturesListsController(IUserOwnFeaturesListService svc)
        {
            _svc = svc;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserOwnFeaturesListDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _svc.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var list = await _svc.GetByUserIdAsync(userId);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _svc.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserOwnFeaturesListDto dto)
        {
            var updated = await _svc.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _svc.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "Özellik listesi bulunamadı." });
            }
        }
    }
}
