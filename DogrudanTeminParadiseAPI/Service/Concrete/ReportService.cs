using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;
using System.Globalization;
using ZstdSharp.Unsafe;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class ReportService : IReportService
    {
        private readonly MongoDBRepository<ProcurementEntry> _entryRepo;
        private readonly MongoDBRepository<ProcurementListItem> _itemRepo;
        private readonly MongoDBRepository<OfferLetter> _offerRepo;
        private readonly MongoDBRepository<Entreprise> _entRepo;
        private readonly MongoDBRepository<Unit> _unitRepo;
        private readonly MongoDBRepository<User> _userRepo;
        private readonly MongoDBRepository<InspectionAcceptanceCertificate> _inspectionRepo;
        private readonly MongoDBRepository<AdditionalInspectionAcceptanceCertificate> _addInspectionRepo;
        private readonly IInspectionAcceptanceCertificateService _inspectionSvc;
        private readonly IAdditionalInspectionAcceptanceService _addInspectionSvc;
        private readonly IBudgetItemService _budgetItemSvc;

        public ReportService(MongoDBRepository<ProcurementEntry> entryRepo, MongoDBRepository<ProcurementListItem> itemRepo, MongoDBRepository<OfferLetter> offerRepo, MongoDBRepository<Entreprise> entRepo, MongoDBRepository<Unit> unitRepo, MongoDBRepository<InspectionAcceptanceCertificate> inspectionRepo, MongoDBRepository<User> userRepo, MongoDBRepository<AdditionalInspectionAcceptanceCertificate> addInspectionRepo, IInspectionAcceptanceCertificateService inspectionSvc, IAdditionalInspectionAcceptanceService addInspectionSvc, IBudgetItemService budgetItemSvc)
        {
            _entryRepo = entryRepo;
            _itemRepo = itemRepo;
            _offerRepo = offerRepo;
            _entRepo = entRepo;
            _unitRepo = unitRepo;
            _userRepo = userRepo;
            _inspectionRepo = inspectionRepo;
            _addInspectionRepo = addInspectionRepo;
            _inspectionSvc = inspectionSvc;
            _addInspectionSvc = addInspectionSvc;
            _budgetItemSvc = budgetItemSvc;

        }

        public async Task<ApproximateCostScheduleDto> GetApproximateCostScheduleAsync(Guid procurementEntryId)
        {
            var entry = await _entryRepo.GetByIdAsync(procurementEntryId)
                ?? throw new KeyNotFoundException("Procurement entry not found.");

            var items = (await _itemRepo.GetAllAsync())
                .Where(i => i.ProcurementEntryId == procurementEntryId)
                .ToList();

            var offers = (await _offerRepo.GetAllAsync())
                .Where(o => o.ProcurementEntryId == procurementEntryId)
                .ToList();

            var dto = new ApproximateCostScheduleDto
            {
                ProcurementEntryName = entry.WorkName,
                ProcurementDecisionDate = entry.ProcurementDecisionDate,
                Items = new List<ItemCostDto>()
            };

            double totalSum = 0;
            foreach (var item in items)
            {
                var bids = offers
                    .SelectMany(o => o.OfferItems
                        .Where(fi => fi.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
                        .Select(fi => new BidDto
                        {
                            EntrepriseId = o.EntrepriseId,
                            UnitPrice = fi.UnitPrice,
                            TotalPrice = fi.TotalAmount
                        }))
                    .ToList();

                double avgUnit = bids.Any() ? bids.Average(b => b.UnitPrice) : 0;
                double avgTotal = avgUnit * item.Quantity;
                totalSum += avgTotal;

                var unit = await _unitRepo.GetByIdAsync(item.UnitId);

                dto.Items.Add(new ItemCostDto
                {
                    ItemName = item.Name,
                    Quantity = item.Quantity,
                    UnitName = unit?.Name,
                    Bids = bids,
                    AverageUnitPrice = avgUnit,
                    AverageTotalCost = avgTotal
                });
            }

            dto.AverageTotalCostSum = totalSum;
            return dto;
        }

        public async Task<MarketPriceResearchReportDto> GetMarketPriceResearchReportAsync(Guid procurementEntryId)
        {
            var entry = await _entryRepo.GetByIdAsync(procurementEntryId)
                ?? throw new KeyNotFoundException("Procurement entry not found.");

            var offers = (await _offerRepo.GetAllAsync())
                .Where(o => o.ProcurementEntryId == procurementEntryId)
                .ToList();
            if (!offers.Any())
                throw new InvalidOperationException("No offer letters for entry.");

            // 1) En düşük ortalama birim fiyatı veren offer'ı seç
            var winningOffer = offers
                .OrderBy(o =>
                    o.OfferItems.Any()
                        ? o.OfferItems.Average(fi => fi.UnitPrice)
                        : double.MaxValue
                )
                .First();

            // 2) Kazanan offer'ın toplam teklif tutarını hesapla
            double totalOfferedPrice = winningOffer.OfferItems.Any()
                ? winningOffer.OfferItems.Sum(fi => fi.TotalAmount)
                : 0;

            // 3) Entreprise bilgisi
            var ent = await _entRepo.GetByIdAsync(winningOffer.EntrepriseId)
                ?? throw new KeyNotFoundException("Winning entreprise not found.");

            // 4) DTO'yu oluştur ve döndür
            return new MarketPriceResearchReportDto
            {
                WorkReason = entry.WorkReason ?? "",
                ProcurementEntryName = entry.WorkName ?? "",
                PiyasaArastirmaBaslangicDate = entry.PiyasaArastirmaBaslangicDate,
                ProcurementDecisionNumber = entry.ProcurementDecisionNumber ?? "",
                PiyasaArastirmaBaslangicNumber = entry.PiyasaArastirmaBaslangicNumber ?? "",
                ProcurementDecisionDate = entry.ProcurementDecisionDate,

                WinnerEntreprise = new WinnerDto
                {
                    Vkn = ent.Vkn,
                    Unvan = ent.Unvan,
                    TotalOfferedPrice = totalOfferedPrice
                }
            };
        }

        public async Task<InspectionAcceptanceReportDto> GetInspectionAcceptanceReportAsync(Guid procurementEntryId, DateTime invoiceDate, string invoiceNumber)
        {
            var entry = await _entryRepo.GetByIdAsync(procurementEntryId)
                ?? throw new KeyNotFoundException("Procurement entry not found.");

            var itemEntities = (await _itemRepo.GetAllAsync())
                .Where(i => i.ProcurementEntryId == procurementEntryId)
                .ToList();

            var itemDtos = await Task.WhenAll(itemEntities.Select(async i => new InspectionItemDto
            {
                Name = i.Name,
                Quantity = i.Quantity,
                UnitName = (await _unitRepo.GetByIdAsync(i.UnitId))?.Name ?? "-"
            }));

            return new InspectionAcceptanceReportDto
            {
                ProcurementDecisionDate = entry.ProcurementDecisionDate,
                ProcurementDecisionNumber = entry.ProcurementDecisionNumber,
                InvoiceDate = invoiceDate,
                InvoiceNumber = invoiceNumber,
                Items = itemDtos.ToList()
            };
        }

        public async Task<List<TopUnitDto>> GetTopBudgetAllocationsAsync(Guid tenderResponsibleUserId, int top)
        {
            // 1) İlgili kullanıcının ProcurementEntry’lerini al
            var allEntries = await _entryRepo.GetAllAsync();
            var userEntries = allEntries
                .Where(e => e.TenderResponsibleUserId == tenderResponsibleUserId && e.BudgetAllocationId.HasValue)
                .ToList();

            if (!userEntries.Any())
                return new List<TopUnitDto>();

            // 2) BudgetAllocationId bazında grupla ve say
            var counts = userEntries
                .GroupBy(e => e.BudgetAllocationId.Value)
                .Select(g => new { BudgetId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            // 3) En çok alan ilk ‘top’ kadar, gerisini “Diğer” grubuna ekle
            var topList = counts.Take(top).ToList();
            var otherCount = counts.Skip(top).Sum(x => x.Count);

            var result = new List<TopUnitDto>();
            foreach (var item in topList)
            {
                // BudgetItemService’den adını al
                var budgetItem = await _budgetItemSvc.GetByIdAsync(item.BudgetId);
                var name = budgetItem?.Name ?? "Bilinmeyen";

                result.Add(new TopUnitDto
                {
                    UnitName = name,
                    Count = item.Count
                });
            }

            if (otherCount > 0)
            {
                result.Add(new TopUnitDto
                {
                    UnitName = "Diğer",
                    Count = otherCount
                });
            }

            return result;
        }

        public async Task<InspectionPriceStatsDto> GetInspectionPriceSumAsync(int days)
        {
            var end = DateTime.UtcNow.Date;
            var start = end.AddDays(-days + 1);

            // fetch normal inspection certificates
            var normalList = await _inspectionRepo.GetAllAsync();
            var normals = normalList
                .Where(c => c.InvoiceDate.Date >= start && c.InvoiceDate.Date <= end)
                .ToList();

            // fetch additional inspection certificates
            var additionalList = await _addInspectionRepo.GetAllAsync();
            var additionals = additionalList
                .Where(c => c.InvoiceDate.Date >= start && c.InvoiceDate.Date <= end)
                .ToList();

            double total = 0;
            // sum normal
            foreach (var cert in normals)
            {
                total += cert.SelectedProducts.Sum(p => p.UnitPrice * p.Quantity);
            }
            // sum additional
            foreach (var cert in additionals)
            {
                total += cert.SelectedProducts.Sum(p => p.UnitPrice * p.Quantity);
            }

            return new InspectionPriceStatsDto
            {
                Days = days,
                TotalPrice = total
            };
        }

        public async Task<List<ProductPriceStatDto>> GetTopInspectionProductsAsync(int days, int top)
        {
            var end = DateTime.UtcNow.Date;
            var start = end.AddDays(-days + 1);

            var normals = (await _inspectionRepo.GetAllAsync())
                .Where(c => c.InvoiceDate.Date >= start && c.InvoiceDate.Date <= end)
                .SelectMany(c => c.SelectedProducts);

            var additionals = (await _addInspectionRepo.GetAllAsync())
                .Where(c => c.InvoiceDate.Date >= start && c.InvoiceDate.Date <= end)
                .SelectMany(c => c.SelectedProducts);

            var allItems = normals.Concat(additionals);

            var grouped = allItems
                .GroupBy(i => i.Name)
                .Select(g => new ProductPriceStatDto
                {
                    Name = g.Key,
                    TotalPrice = g.Sum(p => p.UnitPrice * p.Quantity)
                })
                .OrderByDescending(x => x.TotalPrice)
                .Take(top)
                .ToList();

            return grouped;
        }

        public async Task<List<FirmStatDto>> GetTopInspectionFirmsMonthlyAsync(int top)
        {
            // last 30 days
            var end = DateTime.UtcNow.Date;
            var start = end.AddDays(-29);

            // normal certificates
            var normals = (await _inspectionRepo.GetAllAsync())
                .Where(c => c.InvoiceDate.Date >= start && c.InvoiceDate.Date <= end)
                .ToList();

            // additional certificates
            var additionals = (await _addInspectionRepo.GetAllAsync())
                .Where(c => c.InvoiceDate.Date >= start && c.InvoiceDate.Date <= end)
                .ToList();

            // collect entreprise ids
            var firmIds = new List<Guid>();

            // normals: derive firm via winner of entry
            foreach (var cert in normals)
            {
                var offers = (await _offerRepo.GetAllAsync())
                    .Where(o => o.ProcurementEntryId == cert.ProcurementEntryId)
                    .ToList();
                if (!offers.Any()) continue;
                var winner = offers
                    .OrderBy(o => o.OfferItems.Any() ? o.OfferItems.Average(fi => fi.UnitPrice) : double.MaxValue)
                    .First();
                firmIds.Add(winner.EntrepriseId);
            }

            // additionals: have SelectedOfferLetterId
            foreach (var cert in additionals)
            {
                var offer = await _offerRepo.GetByIdAsync(cert.SelectedOfferLetterId);
                if (offer != null) firmIds.Add(offer.EntrepriseId);
            }

            // group by firm
            var grouped = firmIds
                .GroupBy(id => id)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(top)
                .ToList();

            // fetch entreprise details and map
            var result = new List<FirmStatDto>();
            foreach (var item in grouped)
            {
                var ent = await _entRepo.GetByIdAsync(item.Id);
                if (ent == null) continue;
                result.Add(new FirmStatDto
                {
                    Vkn = ent.Vkn,
                    Unvan = ent.Unvan,
                    Count = item.Count
                });
            }

            return result;
        }

        public async Task<List<LastJobsDto>> GetLast10JobsAsync(Guid tenderResponsibleId)
        {
            var allEntries = await _entryRepo.GetAllAsync();
            var userEntries = allEntries
                .Where(e => e.TenderResponsibleUserId == tenderResponsibleId)
                .ToList();

            if (!userEntries.Any())
                return [];

            var inspectionTasks = userEntries
                .Select(e => _inspectionSvc.GetAllByEntryAsync(e.Id));
            var additionalTasks = userEntries
                .Select(e => _addInspectionSvc.GetAllByEntryAsync(e.Id));

            var inspectionResults = await Task.WhenAll(inspectionTasks);
            var additionalResults = await Task.WhenAll(additionalTasks);

            var lastJobs = new List<LastJobsDto>();

            foreach (var entry in userEntries)
            {
                var workName = entry.WorkName;

                var inspections = inspectionResults
                    .SelectMany(list => list)
                    .Where(i => i.ProcurementEntryId == entry.Id);

                foreach (var cert in inspections)
                {
                    lastJobs.Add(new LastJobsDto
                    {
                        ProcurementEntryId = cert.ProcurementEntryId,
                        WorkName = workName,
                        InvoiceNumber = cert.InvoiceNumber,
                        InvoiceDate = cert.InvoiceDate
                    });
                }

                var additionals = additionalResults
                    .SelectMany(list => list)
                    .Where(a => a.ProcurementEntryId == entry.Id);

                foreach (var cert in additionals)
                {
                    lastJobs.Add(new LastJobsDto
                    {
                        ProcurementEntryId = cert.ProcurementEntryId,
                        WorkName = workName,
                        InvoiceNumber = cert.InvoiceNumber,
                        InvoiceDate = cert.InvoiceDate
                    });
                }
            }

            return lastJobs
                .OrderByDescending(j => j.InvoiceDate)
                .Take(10)
                .ToList();
        }

        public async Task<List<TopUnitDto>> GetTopAdministrationUnitsAsync(Guid tenderResponsibleId)
        {
            var allEntries = await _entryRepo.GetAllAsync();
            var userEntryIds = allEntries
                .Where(e => e.TenderResponsibleUserId == tenderResponsibleId)
                .Select(e => e.Id)
                .ToHashSet();

            if (!userEntryIds.Any())
                return new List<TopUnitDto>();

            var inspections = await _inspectionRepo.GetAllAsync();
            var additionals = await _addInspectionRepo.GetAllAsync();

            var allCertificates = inspections
                .Where(i => userEntryIds.Contains(i.ProcurementEntryId))
                .Select(i => i.AdministrationUnitId)
                .Concat(additionals
                    .Where(a => userEntryIds.Contains(a.ProcurementEntryId))
                    .Select(a => a.AdministrationUnitId));

            var counts = allCertificates
                .GroupBy(id => id)
                .Select(g => new { UnitId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            if (!counts.Any())
                return new List<TopUnitDto>();

            var topFive = counts.Take(5).ToList();
            var otherCount = counts.Skip(5).Sum(x => x.Count);

            var result = new List<TopUnitDto>();
            foreach (var item in topFive)
            {
                // UnitName burada zaten dto’da doldurulacak controller’da
                result.Add(new TopUnitDto
                {
                    UnitName = item.UnitId.ToString(),
                    Count = item.Count
                });
            }

            if (otherCount > 0)
            {
                result.Add(new TopUnitDto
                {
                    UnitName = "Diğer",
                    Count = otherCount
                });
            }

            return result;
        }

        public async Task<List<TopUnitDto>> GetTopSubAdministrationUnitsAsync(Guid tenderResponsibleId)
        {
            var allEntries = await _entryRepo.GetAllAsync();
            var userEntryIds = allEntries
                .Where(e => e.TenderResponsibleUserId == tenderResponsibleId)
                .Select(e => e.Id)
                .ToHashSet();

            if (!userEntryIds.Any())
                return new List<TopUnitDto>();

            var inspections = await _inspectionRepo.GetAllAsync();
            var additionals = await _addInspectionRepo.GetAllAsync();

            var allCertificates = inspections
                .Where(i => userEntryIds.Contains(i.ProcurementEntryId))
                .Select(i => i.SubAdministrationUnitId)
                .Concat(additionals
                    .Where(a => userEntryIds.Contains(a.ProcurementEntryId))
                    .Select(a => a.SubAdministrationUnitId));

            var counts = allCertificates
                .GroupBy(id => id)
                .Select(g => new { UnitId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            if (!counts.Any())
                return new List<TopUnitDto>();

            var topFive = counts.Take(5).ToList();
            var otherCount = counts.Skip(5).Sum(x => x.Count);

            var result = new List<TopUnitDto>();
            foreach (var item in topFive)
            {
                result.Add(new TopUnitDto
                {
                    UnitName = item.UnitId.ToString(),
                    Count = item.Count
                });
            }

            if (otherCount > 0)
            {
                result.Add(new TopUnitDto
                {
                    UnitName = "Diğer",
                    Count = otherCount
                });
            }

            return result;
        }

        public async Task<List<TopUnitDto>> GetTopThreeSubAdministrationUnitsAsync(Guid tenderResponsibleId)
        {
            var allEntries = await _entryRepo.GetAllAsync();
            var userEntryIds = allEntries
                .Where(e => e.TenderResponsibleUserId == tenderResponsibleId)
                .Select(e => e.Id)
                .ToHashSet();

            if (!userEntryIds.Any())
                return new List<TopUnitDto>();

            var inspections = await _inspectionRepo.GetAllAsync();
            var additionals = await _addInspectionRepo.GetAllAsync();

            var allCertificates = inspections
                .Where(i => userEntryIds.Contains(i.ProcurementEntryId))
                .Select(i => i.ThreeSubAdministrationUnitId)
                .Concat(additionals
                    .Where(a => userEntryIds.Contains(a.ProcurementEntryId))
                    .Select(a => a.ThreeSubAdministrationUnitId));

            var counts = allCertificates
                .GroupBy(id => id)
                .Select(g => new { UnitId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            if (!counts.Any())
                return new List<TopUnitDto>();

            var topFive = counts.Take(5).ToList();
            var otherCount = counts.Skip(5).Sum(x => x.Count);

            var result = new List<TopUnitDto>();
            foreach (var item in topFive)
            {
                result.Add(new TopUnitDto
                {
                    UnitName = item.UnitId.ToString(),
                    Count = item.Count
                });
            }

            if (otherCount > 0)
            {
                result.Add(new TopUnitDto
                {
                    UnitName = "Diğer",
                    Count = otherCount
                });
            }

            return result;
        }

        public async Task<SpendingReportDto> GetSpendingReportAsync(Guid tenderResponsibleId)
        {
            var allEntries = await _entryRepo.GetAllAsync();
            var userEntries = allEntries
                .Where(e => e.TenderResponsibleUserId == tenderResponsibleId)
                .ToList();

            if (!userEntries.Any())
                return new SpendingReportDto
                {
                    Weekly = [],
                    Monthly = [],
                    Quarterly = [],
                    Yearly = []
                };

            var entryIdSet = userEntries.Select(e => e.Id).ToHashSet();

            var inspections = await _inspectionRepo.GetAllAsync();
            var additionals = await _addInspectionRepo.GetAllAsync();

            var relevantInspections = inspections
                .Where(i => entryIdSet.Contains(i.ProcurementEntryId))
                .ToList();
            var relevantAdditionals = additionals
                .Where(a => entryIdSet.Contains(a.ProcurementEntryId))
                .ToList();

            var records = new List<(DateTime Date, double Amount)>();

            foreach (var cert in relevantInspections)
            {
                var total = cert.SelectedProducts.Sum(p => p.UnitPrice * p.Quantity);
                records.Add((cert.InvoiceDate.Date, total));
            }

            foreach (var cert in relevantAdditionals)
            {
                var total = cert.SelectedProducts.Sum(p => p.UnitPrice * p.Quantity);
                records.Add((cert.InvoiceDate.Date, total));
            }

            if (!records.Any())
                return new SpendingReportDto
                {
                    Weekly = [],
                    Monthly = [],
                    Quarterly = [],
                    Yearly = []
                };

            // Haftalık
            var weeklyGroups = records
                .GroupBy(r =>
                {
                    var ci = CultureInfo.CurrentCulture;
                    var week = ci.Calendar.GetWeekOfYear(r.Date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                    return (r.Date.Year, Week: week);
                })
                .Select(g =>
                {
                    var firstDay = FirstDateOfWeekISO8601(g.Key.Year, g.Key.Week);
                    return new TimeSeriesDataDto
                    {
                        Period = firstDay.ToString("yyyy-MM-dd"),
                        Total = g.Sum(x => x.Amount)
                    };
                })
                .OrderBy(x => x.Period)
                .ToList();

            // Aylık
            var monthlyGroups = records
                .GroupBy(r => new { r.Date.Year, r.Date.Month })
                .Select(g => new TimeSeriesDataDto
                {
                    Period = $"{g.Key.Month:00}-{g.Key.Year}",
                    Total = g.Sum(x => x.Amount)
                })
                .OrderBy(x =>
                {
                    var parts = x.Period.Split('-');
                    return new DateTime(int.Parse(parts[1]), int.Parse(parts[0]), 1);
                })
                .ToList();

            // Çeyreklik
            var quarterlyGroups = records
                .GroupBy(r =>
                {
                    var quarter = (r.Date.Month - 1) / 3 + 1;
                    return new { r.Date.Year, Quarter = quarter };
                })
                .Select(g => new TimeSeriesDataDto
                {
                    Period = $"Q{g.Key.Quarter}-{g.Key.Year}",
                    Total = g.Sum(x => x.Amount)
                })
                .OrderBy(x =>
                {
                    var parts = x.Period.Split('-', StringSplitOptions.RemoveEmptyEntries);
                    var q = int.Parse(parts[0].TrimStart('Q'));
                    var y = int.Parse(parts[1]);
                    return (y, q);
                })
                .ToList();

            // Yıllık
            var yearlyGroups = records
                .GroupBy(r => r.Date.Year)
                .Select(g => new TimeSeriesDataDto
                {
                    Period = g.Key.ToString(),
                    Total = g.Sum(x => x.Amount)
                })
                .OrderBy(x => int.Parse(x.Period))
                .ToList();

            return new SpendingReportDto
            {
                Weekly = weeklyGroups,
                Monthly = monthlyGroups,
                Quarterly = quarterlyGroups,
                Yearly = yearlyGroups
            };
        }

        public async Task<SpendingByFirmDto> GetTopFirmsSpendingAsync(string periodType)
        {
            // 1) Dönem tipine göre anahtarları (period keys) oluştur
            var now = DateTime.UtcNow;
            List<string> periodKeys = new();
            List<(DateTime start, DateTime end)> ranges = new();

            switch (periodType.ToLower())
            {
                case "weekly":
                    // Son 12 hafta
                    for (int i = 11; i >= 0; i--)
                    {
                        var weekStart = now.Date.AddDays(-7 * i).StartOfWeek(DayOfWeek.Monday);
                        var weekEnd = weekStart.AddDays(6);
                        ranges.Add((weekStart, weekEnd));
                        periodKeys.Add($"W{CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(weekStart, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)} '{weekStart:yy}");
                    }
                    break;

                case "quarterly":
                    // Son 12 çeyrek
                    var currentQuarter = (now.Month - 1) / 3 + 1;
                    for (int i = 11; i >= 0; i--)
                    {
                        var q = currentQuarter - i;
                        var yearOffset = 0;
                        while (q <= 0)
                        {
                            q += 4;
                            yearOffset++;
                        }
                        var quarterYear = now.AddYears(-yearOffset).Year;
                        var startMonth = (q - 1) * 3 + 1;
                        var qStart = new DateTime(quarterYear, startMonth, 1);
                        var qEnd = qStart.AddMonths(3).AddDays(-1);
                        ranges.Add((qStart, qEnd));
                        periodKeys.Add($"Q{q} {quarterYear}");
                    }
                    break;

                case "yearly":
                    // Son 12 yıl
                    for (int i = 11; i >= 0; i--)
                    {
                        var year = now.Year - i;
                        var yStart = new DateTime(year, 1, 1);
                        var yEnd = new DateTime(year, 12, 31);
                        ranges.Add((yStart, yEnd));
                        periodKeys.Add(year.ToString());
                    }
                    break;

                case "monthly":
                default:
                    // Son 12 ay
                    for (int i = 11; i >= 0; i--)
                    {
                        var monthStart = new DateTime(now.Year, now.Month, 1).AddMonths(-i);
                        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                        ranges.Add((monthStart, monthEnd));
                        periodKeys.Add(monthStart.ToString("MMM yyyy", CultureInfo.CurrentCulture));
                    }
                    break;
            }

            // 2) Tüm sertifikaları al
            var inspections = (await _inspectionRepo.GetAllAsync()).ToList();
            var additionals = (await _addInspectionRepo.GetAllAsync()).ToList();
            var allOffers = (await _offerRepo.GetAllAsync()).ToList();

            // 3) Sertifikalardan (Inspection + Additional) toplam harcamayı hesapla
            var combinedCerts = inspections.Select(c => new
            {
                c.InvoiceDate,
                c.SelectedOfferLetterId,
                Total = c.SelectedProducts.Sum(p => p.UnitPrice * p.Quantity)
            })
            .Concat(additionals.Select(c => new
            {
                c.InvoiceDate,
                c.SelectedOfferLetterId,
                Total = c.SelectedProducts.Sum(p => p.UnitPrice * p.Quantity)
            }))
            .ToList();

            // 4) Dönemlere düşenleri firma bazında grupla
            var spendByFirmPeriod = new Dictionary<Guid, Dictionary<string, double>>();
            foreach (var cert in combinedCerts)
            {
                // Dönem aralığına hangi key düşer?
                var periodKey = FindPeriodKey(cert.InvoiceDate, ranges, periodKeys);
                if (string.IsNullOrEmpty(periodKey))
                    continue;

                // İlgili OfferLetter’dan EntrepriseId al
                var offer = allOffers.FirstOrDefault(o => o.Id == cert.SelectedOfferLetterId);
                if (offer == null) continue;
                var firmId = offer.EntrepriseId;

                if (!spendByFirmPeriod.ContainsKey(firmId))
                    spendByFirmPeriod[firmId] = periodKeys.ToDictionary(pk => pk, pk => 0.0);

                spendByFirmPeriod[firmId][periodKey] += cert.Total;
            }

            // 5) Firma toplamlarını hesapla, en çok 5, kalan “Diğer”
            var firmTotals = spendByFirmPeriod
                .Select(kvp => new { FirmId = kvp.Key, Total = kvp.Value.Values.Sum() })
                .OrderByDescending(x => x.Total)
                .ToList();

            var top5 = firmTotals.Take(5).Select(x => x.FirmId).ToList();
            var otherIds = firmTotals.Skip(5).Select(x => x.FirmId).ToList();

            // 6) Series oluştur
            var series = new List<ChartSeriesDto>();

            // En çok 5 firma
            foreach (var firmId in top5)
            {
                var firm = await _entRepo.GetByIdAsync(firmId);
                var name = firm?.Unvan ?? "Bilinmeyen";

                var data = periodKeys
                    .Select(pk => spendByFirmPeriod[firmId].ContainsKey(pk) ? spendByFirmPeriod[firmId][pk] : 0.0)
                    .ToList();

                series.Add(new ChartSeriesDto
                {
                    Name = name,
                    Data = data
                });
            }

            // “Diğer” grubu
            if (otherIds.Any())
            {
                var otherData = periodKeys
                    .Select(pk =>
                        otherIds.Sum(fid =>
                            spendByFirmPeriod.ContainsKey(fid) && spendByFirmPeriod[fid].ContainsKey(pk)
                                ? spendByFirmPeriod[fid][pk]
                                : 0.0
                        )
                    )
                    .ToList();

                series.Add(new ChartSeriesDto
                {
                    Name = "Diğer",
                    Data = otherData
                });
            }

            return new SpendingByFirmDto
            {
                Periods = periodKeys,
                Series = series
            };
        }

        // Tarihi uygun döneme eşleyip anahtarı döndürür
        private static string FindPeriodKey(
            DateTime invoiceDate,
            List<(DateTime start, DateTime end)> ranges,
            List<string> keys)
        {
            for (int i = 0; i < ranges.Count; i++)
            {
                var (start, end) = ranges[i];
                if (invoiceDate.Date >= start.Date && invoiceDate.Date <= end.Date)
                    return keys[i];
            }
            return null;
        }

        public async Task<IEnumerable<UserCountDto>> GetTopResponsibleUsersAsync(int top = 3)
        {
            // Tüm procurement entry'leri al
            var allEntries = await _entryRepo.GetAllAsync();

            // Kullanıcı bazında sayaç oluştur
            var counts = allEntries
                .Where(e => e.TenderResponsibleUserId.HasValue)
                .GroupBy(e => e.TenderResponsibleUserId.Value)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            if (counts.Count == 0)
                return [];

            // İlk top kadar kullanıcı
            var topList = counts.Take(top).ToList();
            var otherCount = counts.Skip(top).Sum(x => x.Count);

            var result = new List<UserCountDto>();
            foreach (var item in topList)
            {
                var user = await _userRepo.GetByIdAsync(item.UserId);
                var name = user != null
                    ? $"{Crypto.Decrypt(user.Name)} {Crypto.Decrypt(user.Surname)}"
                    : "Bilinmeyen";
                result.Add(new UserCountDto
                {
                    UserName = name,
                    Count = item.Count
                });
            }

            if (otherCount > 0)
            {
                result.Add(new UserCountDto
                {
                    UserName = "Diğer",
                    Count = otherCount
                });
            }

            return result;
        }

        public async Task<IEnumerable<UserCountDto>> GetBottomResponsibleUsersAsync(int bottom = 3)
        {
            // Tüm procurement entry'leri al
            var allEntries = await _entryRepo.GetAllAsync();

            // Kullanıcı bazında sayaç oluştur
            var counts = allEntries
                .Where(e => e.TenderResponsibleUserId.HasValue)
                .GroupBy(e => e.TenderResponsibleUserId.Value)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .OrderBy(x => x.Count)
                .ToList();

            if (counts.Count == 0)
                return [];

            // İlk bottom kadar (en az sayıya sahip) kullanıcı
            var bottomList = counts.Take(bottom).ToList();
            var otherCount = counts.Skip(bottom).Sum(x => x.Count);

            var result = new List<UserCountDto>();
            foreach (var item in bottomList)
            {
                var user = await _userRepo.GetByIdAsync(item.UserId);
                var name = user != null
                    ? $"{Crypto.Decrypt(user.Name)} {Crypto.Decrypt(user.Surname)}"
                    : "Bilinmeyen";
                result.Add(new UserCountDto
                {
                    UserName = name,
                    Count = item.Count
                });
            }

            if (otherCount > 0)
            {
                result.Add(new UserCountDto
                {
                    UserName = "Diğer",
                    Count = otherCount
                });
            }

            return result;
        }

        private static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            var jan4 = new DateTime(year, 1, 4);
            var weekday = (int)jan4.DayOfWeek;
            if (weekday == 0) weekday = 7;
            var start = jan4.AddDays(1 - weekday);
            return start.AddDays((weekOfYear - 1) * 7);
        }
    }
}
