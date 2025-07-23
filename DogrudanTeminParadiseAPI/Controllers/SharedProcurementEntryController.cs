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
    public class SharedProcurementEntryController : ControllerBase
    {
        private readonly ISharedProcurementEntryService _svc;
        public SharedProcurementEntryController(ISharedProcurementEntryService svc) => _svc = svc;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSharedProcurementEntryDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByUser), new { userId = created.ProcurementSharerUserId, entryId = created.ProcurementId }, created);
        }

        [HttpGet("user/{userId}/entry/{entryId}")]
        public async Task<IActionResult> GetByUser(Guid userId, Guid entryId)
        {
            var list = await _svc.GetByUserAsync(userId, entryId);
            return Ok(list);
        }

        [HttpDelete("entry/{procurementId}/user/{userId}")]
        public async Task<IActionResult> DeleteUserFromSharers(Guid procurementId, Guid userId)
        {
            try
            {
                await _svc.DeleteUserFromSharersAsync(procurementId, userId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpPut("entry/{procurementId}")]
        public async Task<IActionResult> UpdateSharedToIds(Guid procurementId, [FromBody] UpdateSharedToUserIdsDto dto)
        {
            try
            {
                var updated = await _svc.UpdateSharedToIdsAsync(procurementId, dto.SharedToUserIds);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpDelete("entry/{shareEntryId}")]
        public async Task<IActionResult> Delete(Guid shareEntryId)
        {
            try
            {
                await _svc.DeleteAsync(shareEntryId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}
