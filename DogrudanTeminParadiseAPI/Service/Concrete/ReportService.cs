using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

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

        public ReportService(
            MongoDBRepository<ProcurementEntry> entryRepo,
            MongoDBRepository<ProcurementListItem> itemRepo,
            MongoDBRepository<OfferLetter> offerRepo,
            MongoDBRepository<Entreprise> entRepo,
            MongoDBRepository<Unit> unitRepo,
            MongoDBRepository<InspectionAcceptanceCertificate> inspectionRepo,
            MongoDBRepository<AdditionalInspectionAcceptanceCertificate> addInspectionRepo)
        {
            _entryRepo = entryRepo;
            _itemRepo = itemRepo;
            _offerRepo = offerRepo;
            _entRepo = entRepo;
            _unitRepo = unitRepo;
            _inspectionRepo = inspectionRepo;
            _addInspectionRepo = addInspectionRepo;
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

            // En düşük ortalama birim fiyatı hesapla ve kazanan teklifi al
            var winningOffer = offers
                .OrderBy(o =>
                    o.OfferItems.Any()
                        ? o.OfferItems.Average(fi => fi.UnitPrice)
                        : double.MaxValue
                )
                .First();

            var ent = await _entRepo.GetByIdAsync(winningOffer.EntrepriseId)
                ?? throw new KeyNotFoundException("Winning entreprise not found.");

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
                    Unvan = ent.Unvan
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
    }
}
