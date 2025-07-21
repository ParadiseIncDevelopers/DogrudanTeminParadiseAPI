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
    public class ReportBudgetItemController : ControllerBase
    {
        private readonly IReportBudgetItemService _svc;
        public ReportBudgetItemController(IReportBudgetItemService svc)
        {
            _svc = svc;
        }

        [HttpGet("most-entries")]
        public async Task<IActionResult> GetMostEntries([FromQuery] int top = 3)
        {
            var data = await _svc.GetMostEntryBudgetItemsAsync(top);
            return Ok(data);
        }

        [HttpGet("least-entries")]
        public async Task<IActionResult> GetLeastEntries([FromQuery] int top = 3)
        {
            var data = await _svc.GetLeastEntryBudgetItemsAsync(top);
            return Ok(data);
        }

        [HttpGet("user-entry-extremes")]
        public async Task<IActionResult> GetUserEntryExtremes()
        {
            var data = await _svc.GetUserEntryExtremesAsync();
            return Ok(data);
        }

        [HttpGet("most-paid")]
        public async Task<IActionResult> GetMostPaid([FromQuery] int top = 3)
        {
            var data = await _svc.GetMostPaidBudgetItemsAsync(top);
            return Ok(data);
        }

        [HttpGet("least-paid")]
        public async Task<IActionResult> GetLeastPaid([FromQuery] int top = 3)
        {
            var data = await _svc.GetLeastPaidBudgetItemsAsync(top);
            return Ok(data);
        }

        [HttpGet("offer-totals")]
        public async Task<IActionResult> GetOfferTotals()
        {
            var data = await _svc.GetBudgetItemOfferTotalsAsync();
            return Ok(data);
        }

        [HttpGet("offer-averages")]
        public async Task<IActionResult> GetOfferAverages()
        {
            var data = await _svc.GetBudgetItemOfferAveragesAsync();
            return Ok(data);
        }

        [HttpGet("no-entries")]
        public async Task<IActionResult> GetNoEntries()
        {
            var data = await _svc.GetBudgetItemsWithoutEntriesAsync();
            return Ok(data);
        }
    }
}
