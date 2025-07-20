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
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _svc;
        public NotificationController(INotificationService svc) => _svc = svc;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNotificationDto dto)
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
            var list = await _svc.GetAllByUserIdAsync(userId);
            return Ok(list);
        }

        [HttpDelete("user/{userId}")]
        public async Task<IActionResult> DeleteAll(Guid userId)
        {
            await _svc.DeleteAllAsync(userId);
            return NoContent();
        }

        [HttpPut("user/{userId}/markAllIsRead")]
        public async Task<IActionResult> MarkAllIsRead(Guid userId)
        {
            await _svc.MarkAllIsReadAsync(userId);
            return NoContent();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var dto = await _svc.GetByIdAsync(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _svc.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpPut("{id}/markIsRead")]
        public async Task<IActionResult> MarkIsRead(Guid id)
        {
            try
            {
                var dto = await _svc.MarkIsReadAsync(id);
                return Ok(dto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}
