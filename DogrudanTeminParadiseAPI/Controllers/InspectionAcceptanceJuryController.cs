﻿using DogrudanTeminParadiseAPI.Dto;
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
    public class InspectionAcceptanceJuryController : ControllerBase
    {
        private readonly IInspectionAcceptanceJuryService _svc;
        public InspectionAcceptanceJuryController(IInspectionAcceptanceJuryService svc) => _svc = svc;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInspectionAcceptanceJuryDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("entry/{entryId}")]
        public async Task<IActionResult> GetAllByEntry(Guid entryId) => Ok(await _svc.GetAllByEntryAsync(entryId));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id) => (await _svc.GetByIdAsync(id)) is var dto && dto != null ? Ok(dto) : NotFound();

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInspectionAcceptanceJuryDto dto) => Ok(await _svc.UpdateAsync(id, dto));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _svc.DeleteAsync(id, userId);
            return NoContent();
        }
    }
}
