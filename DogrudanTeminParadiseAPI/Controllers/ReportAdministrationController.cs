using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers.Attributes;
using DogrudanTeminParadiseAPI.Parameters;
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
    public class ReportAdministrationController : ControllerBase
    {
        private readonly IReportAdministrationService _svc;
        private readonly ISuperAdminService _superAdminSvc;

        public ReportAdministrationController(IReportAdministrationService svc, ISuperAdminService superAdminSvc)
        {
            _svc = svc;
            _superAdminSvc = superAdminSvc;
        }

        [HttpGet("most-entry-sub")]
        public async Task<IActionResult> GetMostEntrySub([FromQuery] Guid tenderResponsibleId, [FromQuery] int top = 3)
        {
            var ids = await ResolveTenderResponsibleIds(tenderResponsibleId);
            var data = await _svc.GetMostEntrySubAdministrationUnitsAsync(ids, top);
            return Ok(data);
        }

        [HttpGet("least-entry-sub")]
        public async Task<IActionResult> GetLeastEntrySub([FromQuery] Guid tenderResponsibleId, [FromQuery] int top = 3)
        {
            var ids = await ResolveTenderResponsibleIds(tenderResponsibleId);
            var data = await _svc.GetLeastEntrySubAdministrationUnitsAsync(ids, top);
            return Ok(data);
        }

        [HttpGet("avg-price-sub")]
        public async Task<IActionResult> GetAvgPriceSub([FromQuery] string periodType = "yearly")
        {
            var data = await _svc.GetSubAdministrationAveragePricesAsync(periodType);
            return Ok(data);
        }

        [HttpGet("avg-price-three-sub")]
        public async Task<IActionResult> GetAvgPriceThreeSub([FromQuery] string periodType = "yearly")
        {
            var data = await _svc.GetThreeSubAdministrationAveragePricesAsync(periodType);
            return Ok(data);
        }

        [HttpGet("certificate-count-sub")]
        public async Task<IActionResult> GetCertificateCountSub()
        {
            var data = await _svc.GetSubAdministrationCertificateCountsAsync();
            return Ok(data);
        }

        [HttpGet("certificate-count-three-sub")]
        public async Task<IActionResult> GetCertificateCountThreeSub()
        {
            var data = await _svc.GetThreeSubAdministrationCertificateCountsAsync();
            return Ok(data);
        }

        [HttpGet("offer-count-sub")]
        public async Task<IActionResult> GetOfferCountSub()
        {
            var data = await _svc.GetSubAdministrationOfferCountsAsync();
            return Ok(data);
        }

        [HttpGet("offer-count-three-sub")]
        public async Task<IActionResult> GetOfferCountThreeSub()
        {
            var data = await _svc.GetThreeSubAdministrationOfferCountsAsync();
            return Ok(data);
        }

        [HttpGet("offer-total-sub")]
        public async Task<IActionResult> GetOfferTotalSub()
        {
            var data = await _svc.GetSubAdministrationOfferTotalsAsync();
            return Ok(data);
        }

        [HttpGet("offer-total-three-sub")]
        public async Task<IActionResult> GetOfferTotalThreeSub()
        {
            var data = await _svc.GetThreeSubAdministrationOfferTotalsAsync();
            return Ok(data);
        }

        private async Task<IEnumerable<Guid>> ResolveTenderResponsibleIds(Guid tenderResponsibleId)
        {
            var allGuid = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
            if (tenderResponsibleId != allGuid)
                return new List<Guid> { tenderResponsibleId };

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var adminId))
                throw new UnauthorizedAccessException();

            List<Guid> list;
            try
            {
                list = await _superAdminSvc.GetAdminPermissionsAsync(adminId);
            }
            catch
            {
                list = new List<Guid>();
            }
            list.Insert(0, adminId);
            return list;
        }
    }
}
