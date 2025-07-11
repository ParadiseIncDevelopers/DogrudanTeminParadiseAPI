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
    public class ProductItemController : ControllerBase
    {
        private readonly IProductItemService _svc;
        public ProductItemController(IProductItemService svc) => _svc = svc;

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateProductItemDto dto)
        {
            try { var c = await _svc.CreateAsync(dto); return CreatedAtAction(nameof(GetById), new { id = c.Id }, c); }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpPost("addMass")]
        [Authorize]
        public async Task<IActionResult> AddMass([FromBody] List<CreateProductItemDto> dtos)
        {
            try
            {
                var created = await _svc.AddMassAsync(dtos);
                return Ok(created);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id) =>
            (await _svc.GetByIdAsync(id)) is ProductItemDto dto ? Ok(dto) : NotFound();

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductItemDto dto)
        {
            try { var u = await _svc.UpdateAsync(id, dto); return u == null ? NotFound() : Ok(u); }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            try { await _svc.DeleteAsync(id); return NoContent(); }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        }
    }

}
