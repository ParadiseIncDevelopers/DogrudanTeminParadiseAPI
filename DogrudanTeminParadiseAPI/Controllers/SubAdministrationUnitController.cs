﻿using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers.Attributes;
using DogrudanTeminParadiseAPI.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DogrudanTeminParadiseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [CallLogs]
    public class SubAdministrationUnitController : ControllerBase
    {
        private readonly ISubAdministrationUnitService _svc;

        public SubAdministrationUnitController(ISubAdministrationUnitService svc)
        {
            _svc = svc;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateSubAdministrationUnitDto dto)
        {
            try
            {
                var created = await _svc.CreateAsync(dto);
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
        [Authorize]
        public async Task<IActionResult> GetAll()
            => Ok(await _svc.GetAllAsync());

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _svc.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubAdministrationUnitDto dto)
        {
            try
            {
                var updated = await _svc.UpdateAsync(id, dto);
                return updated == null ? NotFound() : Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _svc.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}
