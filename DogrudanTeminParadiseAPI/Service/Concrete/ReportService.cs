using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;
using System.Globalization;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class ReportService : IReportService
    {
        private readonly MongoDBRepository<ProcurementEntry> _entryRepo;
        private readonly MongoDBRepository<ProcurementListItem> _itemRepo;
        private readonly MongoDBRepository<OfferLetter> _offerRepo;
        private readonly MongoDBRepository<Entreprise> _entRepo;
        private readonly MongoDBRepository<Unit> _unitRepo;
        private readonly MongoDBRepository<InspectionAcceptanceCertificate> _inspectionRepo;
        private readonly MongoDBRepository<AdditionalInspectionAcceptanceCertificate> _addInspectionRepo;
        private readonly IInspectionAcceptanceCertificateService _inspectionSvc;
        private readonly IAdditionalInspectionAcceptanceService _addInspectionSvc;

        public ReportService(MongoDBRepository<ProcurementEntry> entryRepo, MongoDBRepository<ProcurementListItem> itemRepo, MongoDBRepository<OfferLetter> offerRepo, MongoDBRepository<Entreprise> entRepo, MongoDBRepository<Unit> unitRepo, MongoDBRepository<InspectionAcceptanceCertificate> inspectionRepo, MongoDBRepository<AdditionalInspectionAcceptanceCertificate> addInspectionRepo, IInspectionAcceptanceCertificateService inspectionSvc, IAdditionalInspectionAcceptanceService addInspectionSvc)
        {
            _entryRepo = entryRepo;
            _itemRepo = itemRepo;
            _offerRepo = offerRepo;
            _entRepo = entRepo;
            _unitRepo = unitRepo;
            _inspectionRepo = inspectionRepo;
            _addInspectionRepo = addInspectionRepo;
            _inspectionSvc = inspectionSvc;
            _addInspectionSvc = addInspectionSvc;
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

        public async Task<List<BudgetItemStatsDto>> GetBudgetItemCountsAsync(int days)
        {
            var entries = (await _entryRepo.GetAllAsync())
                .Where(e => e.BudgetAllocationId.HasValue)
                .ToList();

            var end = DateTime.UtcNow.Date;
            var start = end.AddDays(-days + 1);

            // build date range
            var dates = Enumerable.Range(0, days)
                .Select(offset => start.AddDays(offset))
                .ToList();

            // group entries by budget allocation and date
            var grouped = entries
                .Where(e => e.ProcurementDecisionDate.HasValue)
                .Select(e => new
                {
                    Id = e.BudgetAllocationId.Value,
                    e.ProcurementDecisionDate.Value.Date
                })
                .Where(x => x.Date >= start && x.Date <= end)
                .GroupBy(x => new { x.Id, x.Date })
                .ToDictionary(g => (g.Key.Id, g.Key.Date), g => g.Count());

            // get distinct BudgetAllocationIds
            var budgetIds = entries
                .Select(e => e.BudgetAllocationId.Value)
                .Distinct()
                .ToList();

            var result = new List<BudgetItemStatsDto>();
            foreach (var bid in budgetIds)
            {
                var dataPoints = new List<TimeSeriesPointDto>();
                foreach (var date in dates)
                {
                    grouped.TryGetValue((bid, date), out var count);
                    dataPoints.Add(new TimeSeriesPointDto { Date = date, Count = count });
                }
                result.Add(new BudgetItemStatsDto
                {
                    BudgetAllocationId = bid,
                    DataPoints = dataPoints
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
                    Weekly = new List<TimeSeriesDataDto>(),
                    Monthly = new List<TimeSeriesDataDto>(),
                    Quarterly = new List<TimeSeriesDataDto>(),
                    Yearly = new List<TimeSeriesDataDto>()
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
                    Weekly = new List<TimeSeriesDataDto>(),
                    Monthly = new List<TimeSeriesDataDto>(),
                    Quarterly = new List<TimeSeriesDataDto>(),
                    Yearly = new List<TimeSeriesDataDto>()
                };

            // Haftalık
            var weeklyGroups = records
                .GroupBy(r =>
                {
                    var ci = CultureInfo.CurrentCulture;
                    var week = ci.Calendar.GetWeekOfYear(r.Date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                    return (Year: r.Date.Year, Week: week);
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
