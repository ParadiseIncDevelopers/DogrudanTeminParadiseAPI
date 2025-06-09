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
    public class AdditionalInspectionAcceptanceCertificateController : ControllerBase
    {
        private readonly IAdditionalInspectionAcceptanceService _svc;

        public AdditionalInspectionAcceptanceCertificateController(IAdditionalInspectionAcceptanceService svc)
        {
            _svc = svc;
        }

        [HttpPost]
        [PermissionCheck]
        public async Task<IActionResult> Create([FromBody] CreateAdditionalInspectionAcceptanceCertificateDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet]
        [PermissionCheck]
        public async Task<IActionResult> GetAll()
        {
            var permitted = HttpContext.Items["PermittedList"] as IEnumerable<Guid>;
            var list = await _svc.GetAllAsync(permitted);
            return Ok(list);
        }

        [HttpGet("entry/{entryId}")]
        [PermissionCheck]
        public async Task<IActionResult> GetAllByEntry(Guid entryId)
        {
            var permitted = HttpContext.Items["PermittedList"] as IEnumerable<Guid>;
            var list = await _svc.GetAllByEntryAsync(entryId, permitted);
            return Ok(list);
        }

        [HttpGet("{id}")]
        [PermissionCheck]
        public async Task<IActionResult> GetById(Guid id)
        {
            var dto = await _svc.GetByIdAsync(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpPut("{id}")]
        [PermissionCheck]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAdditionalInspectionAcceptanceCertificateDto dto)
        {
            var updated = await _svc.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        [PermissionCheck]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _svc.DeleteAsync(id);
            return NoContent();
        }
    }
}
