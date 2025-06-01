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
    public class ApproximateCostJuryController : ControllerBase
    {
        private readonly IApproximateCostJuryService _svc;
        public ApproximateCostJuryController(IApproximateCostJuryService svc) => _svc = svc;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateApproximateCostJuryDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("entry/{entryId}")]
        public async Task<IActionResult> GetAllByEntry(Guid entryId)
            => Ok(await _svc.GetAllByEntryAsync(entryId));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
            => (await _svc.GetByIdAsync(id)) is var dto && dto != null ? Ok(dto) : NotFound();

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateApproximateCostJuryDto dto)
            => Ok(await _svc.UpdateAsync(id, dto));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _svc.DeleteAsync(id);
            return NoContent();
        }
    }
}
