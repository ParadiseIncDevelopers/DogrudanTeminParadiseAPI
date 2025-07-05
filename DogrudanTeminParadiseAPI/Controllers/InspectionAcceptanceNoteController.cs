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
    public class InspectionAcceptanceNotesController : ControllerBase
    {
        private readonly IInspectionAcceptanceNoteService _svc;
        public InspectionAcceptanceNotesController(IInspectionAcceptanceNoteService svc)
        {
            _svc = svc;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInspectionAcceptanceNoteDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("entry/{entryId}")]
        public async Task<IActionResult> GetAllByEntry(Guid entryId)
        {
            var list = await _svc.GetAllByEntryAsync(entryId);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _svc.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInspectionAcceptanceNoteDto dto)
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
                return NotFound(new { error = "Note bulunamadı." });
            }
        }
    }
}
