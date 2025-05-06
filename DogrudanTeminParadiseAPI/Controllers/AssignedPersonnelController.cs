using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DogrudanTeminParadiseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssignedPersonnelController : ControllerBase
    {
        private readonly IAssignedPersonnelService _svc;
        public AssignedPersonnelController(IAssignedPersonnelService svc) => _svc = svc;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAssignedPersonnelDto dto)
        {
            try
            {
                var created = await _svc.CreateAsync(dto);
                return CreatedAtAction(nameof(GetByEntry), new { procurementEntryId = created.ProcurementEntryId }, created);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpGet("entry/{entryId}")]
        public async Task<IActionResult> GetByEntry(Guid entryId)
        {
            var item = await _svc.GetByEntryAsync(entryId);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPut("entry/{entryId}")]
        public async Task<IActionResult> Update(Guid entryId, [FromBody] CreateAssignedPersonnelDto dto)
        {
            try
            {
                var updated = await _svc.UpdateAsync(entryId, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }
    }
}
