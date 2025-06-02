using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers.Attributes;
using DogrudanTeminParadiseAPI.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DogrudanTeminParadiseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [EnableCors("AllowMyClient")]
    [CallLogs]
    public class ProcurementEntryController : ControllerBase
    {
        private readonly IProcurementEntryService _entrySvc;
        private readonly IOfferLetterService _offerSvc;

        public ProcurementEntryController(IProcurementEntryService entrySvc, IOfferLetterService offerSvc) 
        {
            _entrySvc = entrySvc;
            _offerSvc = offerSvc;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProcurementEntryDto dto)
        {
            try
            {
                var created = await _entrySvc.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
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

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _entrySvc.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _entrySvc.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProcurementEntryDto dto)
        {
            try
            {
                var updated = await _entrySvc.UpdateAsync(id, dto);
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

        [HttpGet("filter-by-price")]
        public async Task<IActionResult> GetInspectionPriceRange([FromQuery] ProcurementEntryInspectionPriceDto query)
        {
            var entries = await _entrySvc.GetInspectionPriceRangeAsync(query);
            return Ok(entries);
        }

        [HttpGet("filter-by-offer-count")]
        public async Task<IActionResult> GetByOfferCount([FromQuery] ProcurementEntryWithOfferCountDto query)
        {
            var entries = await _entrySvc.GetByOfferCountAsync(query);
            return Ok(entries);
        }

        [HttpGet("filter-by-units")]
        public async Task<IActionResult> GetByAdministrativeUnits([FromQuery] ProcurementEntryWithUnitFilterDto query)
        {
            var result = await _entrySvc.GetByAdministrativeUnitsAsync(query);
            return Ok(result);
        }

        [HttpGet("filter-by-vkn")]
        public async Task<IActionResult> GetByVkn([FromQuery] string vkn)
        {
            var result = await _entrySvc.GetByVknAsync(vkn);
            return Ok(result);
        }

        [HttpGet("filter-by-budget")]
        public async Task<IActionResult> GetByBudgetAllocation([FromQuery] Guid budgetAllocationId)
        {
            var entries = await _entrySvc.GetByBudgetAllocationAsync(budgetAllocationId);
            return Ok(entries);
        }

        [HttpGet("filter-by-inspection-date")]
        public async Task<IActionResult> GetByInspectionDateRange([FromQuery] ProcurementEntryDateRangeDto query)
        {
            try
            {
                var entries = await _entrySvc.GetByInspectionDateRangeAsync(query);
                return Ok(entries);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _entrySvc.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpGet("by-requester")]
        public async Task<IActionResult> GetByRequester()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var role = User.FindFirstValue(ClaimTypes.Role);
            var isAdmin = string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);

            var result = await _entrySvc.GetByRequesterAsync(userId, isAdmin);
            return Ok(result);
        }
    }
}
