using DogrudanTeminParadiseAPI.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DogrudanTeminParadiseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _svc;
        public ReportController(IReportService svc) => _svc = svc;

        [HttpGet("approximate-cost/{entryId}")]
        public async Task<IActionResult> GetApproximateCost(Guid entryId)
        {
            try
            {
                var report = await _svc.GetApproximateCostScheduleAsync(entryId);
                return Ok(report);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        }

        [HttpGet("market-price/{entryId}")]
        public async Task<IActionResult> GetMarketPriceReport(Guid entryId)
        {
            try
            {
                var report = await _svc.GetMarketPriceResearchReportAsync(entryId);
                return Ok(report);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpGet("inspection-acceptance/{entryId}")]
        public async Task<IActionResult> GetInspectionAcceptance(Guid entryId, [FromQuery] DateTime invoiceDate, [FromQuery] string invoiceNumber)
        {
            try
            {
                var report = await _svc.GetInspectionAcceptanceReportAsync(
                    entryId, invoiceDate, invoiceNumber);
                return Ok(report);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}
