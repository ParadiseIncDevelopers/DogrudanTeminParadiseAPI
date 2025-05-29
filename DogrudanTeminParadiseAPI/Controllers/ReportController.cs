using DogrudanTeminParadiseAPI.Parameters;
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

        [HttpGet("dashboard/inspection-price-sum")]
        public async Task<IActionResult> GetInspectionPriceSum([FromQuery] StatsQueryParameters q)
        {
            if (q.Days <= 0) return BadRequest(new { error = "Days must be > 0" });
            var stats = await _svc.GetInspectionPriceSumAsync(q.Days);
            return Ok(stats);
        }

        [HttpGet("dashboard/budget-item-stats")]
        public async Task<IActionResult> GetBudgetItemStats([FromQuery] StatsQueryParameters query)
        {
            if (query.Days <= 0)
                return BadRequest(new { error = "Days must be > 0" });

            var stats = await _svc.GetBudgetItemCountsAsync(query.Days);
            return Ok(stats);
        }

        [HttpGet("dashboard/top-inspection-products")]
        public async Task<IActionResult> GetTopInspectionProducts([FromQuery] StatsQueryParameters q)
        {
            if (q.Days <= 0 || q.Top <= 0)
                return BadRequest(new { error = "Days and Top must be > 0" });

            var list = await _svc.GetTopInspectionProductsAsync(q.Days, q.Top);
            return Ok(list);
        }

        [HttpGet("dashboard/top-inspection-firms")]
        public async Task<IActionResult> GetTopInspectionFirms([FromQuery] int top = 5)
        {
            if (top <= 0) return BadRequest(new { error = "Top must be > 0" });
            var list = await _svc.GetTopInspectionFirmsMonthlyAsync(top);
            return Ok(list);
        }
    }
}
