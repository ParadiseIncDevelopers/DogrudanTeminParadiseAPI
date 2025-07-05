using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers.Attributes;
using DogrudanTeminParadiseAPI.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DogrudanTeminParadiseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [CallLogs]
    public class InspectionAcceptanceCertificateController : ControllerBase
    {
        private readonly IInspectionAcceptanceCertificateService _svc;
        public InspectionAcceptanceCertificateController(IInspectionAcceptanceCertificateService svc) => _svc = svc;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInspectionAcceptanceCertificateDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Get all certificates, filtered by permission list
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _svc.GetAllAsync();
            return Ok(list);
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
            var dto = await _svc.GetByIdAsync(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInspectionAcceptanceCertificateDto dto)
        {
            var updated = await _svc.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _svc.DeleteAsync(id, userId);
            return NoContent();
        }
    }
}
