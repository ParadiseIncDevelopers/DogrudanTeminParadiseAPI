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
    public class OSSharedProcurementEntryController : ControllerBase
    {
        private readonly IOSSharedProcurementEntryService _svc;
        public OSSharedProcurementEntryController(IOSSharedProcurementEntryService svc) => _svc = svc;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOSSharedProcurementEntryDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByUser), new { userId = created.ProcurementSharerUserId, entryId = created.OneSourceProcurementEntryId }, created);
        }

        [HttpGet("user/{userId}/entry/{entryId}")]
        public async Task<IActionResult> GetByUser(Guid userId, Guid entryId)
        {
            var list = await _svc.GetByUserAsync(userId, entryId);
            return Ok(list);
        }

        [HttpDelete("entry/{entryId}/user/{userId}")]
        public async Task<IActionResult> DeleteUserFromSharers(Guid entryId, Guid userId)
        {
            try
            {
                await _svc.DeleteUserFromSharersAsync(entryId, userId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpPut("entry/{entryId}")]
        public async Task<IActionResult> UpdateSharedToIds(Guid entryId, [FromBody] UpdateSharedToUserIdsDto dto)
        {
            try
            {
                var updated = await _svc.UpdateSharedToIdsAsync(entryId, dto.SharedToUserIds);
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
