using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DogrudanTeminParadiseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OSProcurementEntryDocumentsController : ControllerBase
    {
        private readonly IOSProcurementEntryDocumentsService _svc;
        public OSProcurementEntryDocumentsController(IOSProcurementEntryDocumentsService svc) => _svc = svc;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOSProcurementEntryDocumentsDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("entry/{entryId}")]
        public async Task<IActionResult> GetAllByEntry(Guid entryId)
            => Ok(await _svc.GetAllByEntryAsync(entryId));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var doc = await _svc.GetByIdAsync(id);
            return doc == null ? NotFound() : Ok(doc);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOSProcurementEntryDocumentsDto dto)
        {
            var updated = await _svc.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _svc.DeleteAsync(id);
            return NoContent();
        }
    }
}
