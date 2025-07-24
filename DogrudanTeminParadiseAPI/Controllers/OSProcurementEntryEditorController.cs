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
    public class OSProcurementEntryEditorController : ControllerBase
    {
        private readonly IOSProcurementEntryEditorService _svc;
        public OSProcurementEntryEditorController(IOSProcurementEntryEditorService svc) => _svc = svc;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var dto = await _svc.GetAsync();
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var dto = await _svc.GetByIdAsync(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpGet("entry/{entryId}")]
        public async Task<IActionResult> GetByEntry(Guid entryId)
        {
            var dto = await _svc.GetEditorByEntryIdAsync(entryId);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOSProcurementEntryEditorDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateOSProcurementEntryEditorDto dto)
        {
            var updated = await _svc.UpdateAsync(dto);
            return updated == null ? NotFound() : Ok(updated);
        }
    }
}
