using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DogrudanTeminParadiseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcurementEntryEditorController : ControllerBase
    {
        private readonly IProcurementEntryEditorService _svc;
        public ProcurementEntryEditorController(IProcurementEntryEditorService svc) => _svc = svc;

        // Herkes görebilir
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var dto = await _svc.GetAsync();
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var dto = await _svc.GetByIdAsync(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpGet("entry/{entryId}")]
        [Authorize]
        public async Task<IActionResult> GetOfferProductsByEntry(Guid entryId)
        {
            var items = await _svc.GetEditorByEntryIdAsync(entryId);
            return items == null ? NotFound() : Ok(items);
        }

        // Yalnızca Admin rolü ekleyebilir ve güncelleyebilir
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProcurementEntryEditorDto dto)
        {
            try
            {
                var created = await _svc.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateProcurementEntryEditorDto dto)
        {
            var updated = await _svc.UpdateAsync(dto);
            return updated == null ? NotFound() : Ok(updated);
        }
    }
}
