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
    public class SharedProcurementEntryController : ControllerBase
    {
        private readonly ISharedProcurementEntryService _svc;
        public SharedProcurementEntryController(ISharedProcurementEntryService svc) => _svc = svc;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSharedProcurementEntryDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByUser), new { userId = created.ProcurementSharerUserId }, created);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            var list = await _svc.GetByUserAsync(userId);
            return Ok(list);
        }

    }
}
