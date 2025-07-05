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
    public class OfferLetterController : ControllerBase
    {
        private readonly IOfferLetterService _svc;
        public OfferLetterController(IOfferLetterService svc) => _svc = svc;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOfferLetterDto dto)
        {
            try
            {
                var created = await _svc.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

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
            var item = await _svc.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOfferLetterDto dto)
        {
            try
            {
                var updated = await _svc.UpdateAsync(id, dto);
                return updated == null ? NotFound() : Ok(updated);
            }
            catch (KeyNotFoundException ex) 
            { 
                return NotFound(new { error = ex.Message }); 
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("entry/{entryId}/items")]
        public async Task<IActionResult> UpdateItemsByEntry(Guid entryId, [FromBody] UpdateOfferItemsByEntryDto dto)
        {
            try
            {
                var updated = await _svc.UpdateItemsByEntryAsync(entryId, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier));
            try
            {
                await _svc.DeleteAsync(id, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}
