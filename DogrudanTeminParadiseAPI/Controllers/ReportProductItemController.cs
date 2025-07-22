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
    public class ReportProductItemController : ControllerBase
    {
        private readonly IReportProductItemService _svc;
        public ReportProductItemController(IReportProductItemService svc)
        {
            _svc = svc;
        }

        [HttpGet("spending-by-type")]
        public async Task<IActionResult> GetSpendingByType()
        {
            var data = await _svc.GetSpendingByTypeAsync();
            return Ok(data);
        }

        [HttpGet("most-used-products")]
        public async Task<IActionResult> GetMostUsedProducts([FromQuery] int top = 3)
        {
            var data = await _svc.GetMostUsedInProductsAsync(top);
            return Ok(data);
        }

        [HttpGet("least-used-products")]
        public async Task<IActionResult> GetLeastUsedProducts([FromQuery] int top = 3)
        {
            var data = await _svc.GetLeastUsedInProductsAsync(top);
            return Ok(data);
        }

        [HttpGet("most-used-offers")]
        public async Task<IActionResult> GetMostUsedOffers([FromQuery] int top = 3)
        {
            var data = await _svc.GetMostUsedInOffersAsync(top);
            return Ok(data);
        }

        [HttpGet("least-used-offers")]
        public async Task<IActionResult> GetLeastUsedOffers([FromQuery] int top = 3)
        {
            var data = await _svc.GetLeastUsedInOffersAsync(top);
            return Ok(data);
        }

        [HttpGet("firm-offer-extremes")]
        public async Task<IActionResult> GetFirmOfferExtremes([FromQuery] Guid firmId)
        {
            var data = await _svc.GetFirmOfferExtremesAsync(firmId);
            return Ok(data);
        }
    }
}
