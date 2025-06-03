using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers.Attributes;
using DogrudanTeminParadiseAPI.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DogrudanTeminParadiseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [CallLogs]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _svc;
        public CategoryController(ICategoryService svc) => _svc = svc;

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            try { var c = await _svc.CreateAsync(dto); return CreatedAtAction(nameof(GetById), new { id = c.Id }, c); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
            => (await _svc.GetByIdAsync(id)) is CategoryDto dto ? Ok(dto) : NotFound();

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryDto dto)
        {
            try { var u = await _svc.UpdateAsync(id, dto); return u == null ? NotFound() : Ok(u); }
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
