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
    public class ReportProductController : ControllerBase
    {
        private readonly IReportProductService _svc;
        public ReportProductController(IReportProductService svc)
        {
            _svc = svc;
        }

        [HttpGet("most-used")]
        public async Task<IActionResult> GetMostUsed([FromQuery] int top = 3)
        {
            var data = await _svc.GetMostUsedProductsAsync(top);
            return Ok(data);
        }

        [HttpGet("least-used")]
        public async Task<IActionResult> GetLeastUsed([FromQuery] int top = 3)
        {
            var data = await _svc.GetLeastUsedProductsAsync(top);
            return Ok(data);
        }

        [HttpGet("most-inspected")]
        public async Task<IActionResult> GetMostInspected([FromQuery] int top = 5)
        {
            var data = await _svc.GetMostInspectedProductsAsync(top);
            return Ok(data);
        }

        [HttpGet("least-inspected")]
        public async Task<IActionResult> GetLeastInspected([FromQuery] int top = 5)
        {
            var data = await _svc.GetLeastInspectedProductsAsync(top);
            return Ok(data);
        }

        [HttpGet("highest-offers")]
        public async Task<IActionResult> GetHighestOffers([FromQuery] string productName, [FromQuery] int top = 3)
        {
            var data = await _svc.GetHighestOffersAsync(productName, top);
            return Ok(data);
        }

        [HttpGet("lowest-offers")]
        public async Task<IActionResult> GetLowestOffers([FromQuery] string productName, [FromQuery] int top = 3)
        {
            var data = await _svc.GetLowestOffersAsync(productName, top);
            return Ok(data);
        }

        [HttpGet("purchase-report")]
        public async Task<IActionResult> GetPurchaseReport()
        {
            var report = await _svc.GetPurchaseQuantityReportAsync();
            return Ok(report);
        }
    }
}
