using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers.Attributes;
using DogrudanTeminParadiseAPI.Parameters;
using DogrudanTeminParadiseAPI.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;
using System.Collections.Generic;

namespace DogrudanTeminParadiseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [CallLogs]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _svc;
        private readonly IProcurementEntryService _entrySvc;
        private readonly IInspectionAcceptanceCertificateService _inspectionSvc;
        private readonly IAdditionalInspectionAcceptanceService _additionalInspectionSvc;
        private readonly IAdministrationUnitService _adminUnitSvc;
        private readonly ISubAdministrationUnitService _subAdminSvc;
        private readonly IThreeSubAdministrationUnitService _threeSubAdminSvc;
        private readonly IOfferLetterService _offerSvc;
        private readonly IEntrepriseService _entrepriseSvc;
        private readonly ISuperAdminService _superAdminSvc;

        public ReportController(IReportService svc, IProcurementEntryService entrySvc, IInspectionAcceptanceCertificateService inspectionSvc, IAdditionalInspectionAcceptanceService additionalInspectionSvc, IAdministrationUnitService adminUnitSvc,
            ISubAdministrationUnitService subAdminSvc, IThreeSubAdministrationUnitService threeSubAdminSvc, IOfferLetterService offerSvc, IEntrepriseService entrepriseSvc, ISuperAdminService superAdminSvc)
        {
            _svc = svc;
            _entrySvc = entrySvc;
            _inspectionSvc = inspectionSvc;
            _additionalInspectionSvc = additionalInspectionSvc;
            _adminUnitSvc = adminUnitSvc;
            _subAdminSvc = subAdminSvc;
            _threeSubAdminSvc = threeSubAdminSvc;
            _offerSvc = offerSvc;
            _entrepriseSvc = entrepriseSvc;
            _superAdminSvc = superAdminSvc;
        }

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

        [HttpGet("top-budget-allocations")]
        public async Task<IActionResult> GetTopBudgetAllocations([FromQuery] Guid tenderResponsibleUserId, [FromQuery] int top = 5)
        {
            if (top <= 0)
                return BadRequest(new { error = "Top must be > 0" });

            var ids = await ResolveTenderResponsibleIds(tenderResponsibleUserId);
            var stats = await _svc.GetTopBudgetAllocationsAsync(ids, top);
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

        [HttpGet("last-10-jobs")]
        public async Task<IActionResult> GetLast10Jobs([FromQuery] Guid tenderResponsibleId)
        {
            var ids = await ResolveTenderResponsibleIds(tenderResponsibleId);
            var result = await _svc.GetLast10JobsAsync(ids);
            return Ok(result);
        }

        [HttpGet("top-admin-units")]
        public async Task<IActionResult> GetTopAdministrationUnits([FromQuery] Guid tenderResponsibleId)
        {
            var ids = await ResolveTenderResponsibleIds(tenderResponsibleId);
            var raw = await _svc.GetTopAdministrationUnitsAsync(ids);
            var result = new List<TopUnitDto>();

            // Her UnitId'yi gerçekteki Ad isimleriyle eşle
            foreach (var item in raw)
            {
                var unit = await _adminUnitSvc.GetByIdAsync(Guid.Parse(item.UnitName));
                var name = unit?.Name ?? "Bilinmeyen";
                result.Add(new TopUnitDto
                {
                    UnitName = name,
                    Count = item.Count
                });
            }
            return Ok(result);
        }

        [HttpGet("top-sub-admin-units")]
        public async Task<IActionResult> GetTopSubAdministrationUnits([FromQuery] Guid tenderResponsibleId)
        {
            var ids = await ResolveTenderResponsibleIds(tenderResponsibleId);
            var raw = await _svc.GetTopSubAdministrationUnitsAsync(ids);
            var result = new List<TopUnitDto>();

            foreach (var item in raw)
            {
                if (item.UnitName == "Diğer")
                {
                    result.Add(new TopUnitDto
                    {
                        UnitName = "Diğer",
                        Count = item.Count
                    });
                    continue;
                }

                var unit = await _subAdminSvc.GetByIdAsync(Guid.Parse(item.UnitName));
                var name = unit?.Name ?? "Bilinmeyen";
                result.Add(new TopUnitDto
                {
                    UnitName = name,
                    Count = item.Count
                });
            }

            return Ok(result);
        }

        [HttpGet("top-three-sub-admin-units")]
        public async Task<IActionResult> GetTopThreeSubAdministrationUnits([FromQuery] Guid tenderResponsibleId)
        {
            var ids = await ResolveTenderResponsibleIds(tenderResponsibleId);
            var raw = await _svc.GetTopThreeSubAdministrationUnitsAsync(ids);
            var result = new List<TopUnitDto>();

            foreach (var item in raw)
            {
                if (item.UnitName == "Diğer")
                {
                    result.Add(new TopUnitDto
                    {
                        UnitName = "Diğer",
                        Count = item.Count
                    });
                    continue;
                }

                var unit = await _threeSubAdminSvc.GetByIdAsync(Guid.Parse(item.UnitName));
                var name = unit?.Name ?? "Bilinmeyen";
                result.Add(new TopUnitDto
                {
                    UnitName = name,
                    Count = item.Count
                });
            }

            return Ok(result);
        }

        [HttpGet("spending-report")]
        public async Task<IActionResult> GetSpendingReport(Guid tenderResponsibleId)
        {
            var ids = await ResolveTenderResponsibleIds(tenderResponsibleId);
            var report = await _svc.GetSpendingReportAsync(ids);
            return Ok(report);
        }

        /// <summary>
        /// periodType: "weekly", "monthly", "quarterly", "yearly"
        /// </summary>
        [HttpGet("top-firms-spending")]
        public async Task<IActionResult> GetTopFirmsSpending([FromQuery] string periodType = "yearly")
        {
            var chartData = await _svc.GetTopFirmsSpendingAsync(periodType);
            return Ok(chartData);
        }

        [HttpGet("top-responsible-users")]
        public async Task<IActionResult> GetTopResponsibleUsers([FromQuery] int top = 3)
        {
            if (top <= 0)
                return BadRequest(new { error = "Top must be > 0" });
            var data = await _svc.GetTopResponsibleUsersAsync(top);
            return Ok(data);
        }

        [HttpGet("bottom-responsible-users")]
        public async Task<IActionResult> GetBottomResponsibleUsers([FromQuery] int bottom = 3)
        {
            if (bottom <= 0)
                return BadRequest(new { error = "Bottom must be > 0" });
            var data = await _svc.GetBottomResponsibleUsersAsync(bottom);
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
