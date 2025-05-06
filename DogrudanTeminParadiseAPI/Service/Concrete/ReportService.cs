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

        public ReportService(
            MongoDBRepository<ProcurementEntry> entryRepo,
            MongoDBRepository<ProcurementListItem> itemRepo,
            MongoDBRepository<OfferLetter> offerRepo,
            MongoDBRepository<Entreprise> entRepo,
            MongoDBRepository<Unit> unitRepo)
        {
            _entryRepo = entryRepo;
            _itemRepo = itemRepo;
            _offerRepo = offerRepo;
            _entRepo = entRepo;
            _unitRepo = unitRepo;
        }

        public async Task<ApproximateCostScheduleDto> GetApproximateCostScheduleAsync(Guid procurementEntryId)
        {
            // Temin kaydını al
            var entry = await _entryRepo.GetByIdAsync(procurementEntryId)
                ?? throw new KeyNotFoundException("Procurement entry not found.");

            // Alınacak kalemler
            var items = (await _itemRepo.GetAllAsync())
                .Where(i => i.ProcurementEntryId == procurementEntryId)
                .ToList();

            // Tüm teklif mektupları
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
                // Her teklif mektubundan sadece o kaleme ait OfferItem'i al
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

                // Birim adını al
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

            // Her teklif mektubunun ortalama birim fiyatı
            var offerAverages = offers
                .Select(o => new
                {
                    Offer = o,
                    AveragePrice = o.OfferItems.Any()
                        ? o.OfferItems.Average(fi => fi.UnitPrice)
                        : double.MaxValue
                })
                .ToList();

            // Tüm tekliflerin aritmetik ortalaması
            var overallAvg = offerAverages.Average(x => x.AveragePrice);

            // Ortalama veya altında kalanların en yükseğini seç
            var candidates = offerAverages.Where(x => x.AveragePrice <= overallAvg).ToList();
            var winning = candidates.Any()
                ? candidates.OrderByDescending(x => x.AveragePrice).First().Offer
                : offerAverages.OrderBy(x => x.AveragePrice).First().Offer;

            var ent = await _entRepo.GetByIdAsync(winning.EntrepriseId)
                ?? throw new KeyNotFoundException("Winning entreprise not found.");

            return new MarketPriceResearchReportDto
            {
                ProcurementEntryName = entry.WorkName,
                ProcurementDecisionDate = entry.ProcurementDecisionDate,
                WinnerEntreprise = new WinnerDto
                {
                    Vkn = ent.Vkn,
                    Unvan = ent.Unvan
                }
            };
        }

        public async Task<InspectionAcceptanceReportDto> GetInspectionAcceptanceReportAsync(
            Guid procurementEntryId,
            DateTime invoiceDate,
            string invoiceNumber)
        {
            var entry = await _entryRepo.GetByIdAsync(procurementEntryId)
                ?? throw new KeyNotFoundException("Procurement entry not found.");

            //var ent = await _entRepo.GetByIdAsync(entry.ThreeSubAdministrationUnit.AdministrationUnitId)
            //    ?? throw new KeyNotFoundException("Entreprise not found.");

            var dosyaNo = entry.DocumentDatesAndNumbers.FirstOrDefault()?.Number;

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
                //EntrepriseUnvan = ent.Unvan,
                //ThreeSubAdministrationUnitName = entry.ThreeSubAdministrationUnit.Name,
                ProcurementDecisionDate = entry.ProcurementDecisionDate,
                ProcurementDecisionNumber = entry.ProcurementDecisionNumber,
                DosyaNo = dosyaNo,
                InvoiceDate = invoiceDate,
                InvoiceNumber = invoiceNumber,
                Items = [.. itemDtos]
            };
        }
    }
}
