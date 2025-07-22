using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class ReportAdministrationService : IReportAdministrationService
    {
        private readonly MongoDBRepository<ProcurementEntry> _entryRepo;
        private readonly MongoDBRepository<InspectionAcceptanceCertificate> _inspectionRepo;
        private readonly MongoDBRepository<AdditionalInspectionAcceptanceCertificate> _addInspectionRepo;
        private readonly MongoDBRepository<OfferLetter> _offerRepo;
        private readonly MongoDBRepository<SubAdministrationUnit> _subRepo;
        private readonly MongoDBRepository<ThreeSubAdministrationUnit> _threeSubRepo;

        public ReportAdministrationService(
            MongoDBRepository<ProcurementEntry> entryRepo,
            MongoDBRepository<InspectionAcceptanceCertificate> inspectionRepo,
            MongoDBRepository<AdditionalInspectionAcceptanceCertificate> addInspectionRepo,
            MongoDBRepository<OfferLetter> offerRepo,
            MongoDBRepository<SubAdministrationUnit> subRepo,
            MongoDBRepository<ThreeSubAdministrationUnit> threeSubRepo)
        {
            _entryRepo = entryRepo;
            _inspectionRepo = inspectionRepo;
            _addInspectionRepo = addInspectionRepo;
            _offerRepo = offerRepo;
            _subRepo = subRepo;
            _threeSubRepo = threeSubRepo;
        }

        private static double SumCertificate(InspectionAcceptanceCertificate cert)
        {
            return cert.SelectedProducts.Sum(p => p.UnitPrice * p.Quantity);
        }

        private static double SumCertificate(AdditionalInspectionAcceptanceCertificate cert)
        {
            return cert.SelectedProducts.Sum(p => p.UnitPrice * p.Quantity);
        }

        public async Task<IEnumerable<TopUnitDto>> GetMostEntrySubAdministrationUnitsAsync(IEnumerable<Guid> userIds, int top = 3)
        {
            var idSet = userIds?.ToHashSet() ?? new HashSet<Guid>();
            var entries = (await _entryRepo.GetAllAsync())
                .Where(e => e.SubAdministrationUnitId.HasValue &&
                            (!idSet.Any() || (e.TenderResponsibleUserId.HasValue && idSet.Contains(e.TenderResponsibleUserId.Value))))
                .ToList();

            var counts = entries
                .GroupBy(e => e.SubAdministrationUnitId!.Value)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            var topList = counts.Take(top).ToList();
            var other = counts.Skip(top).Sum(x => x.Count);
            var subs = (await _subRepo.GetAllAsync()).ToDictionary(s => s.Id, s => s.Code);
            var result = new List<TopUnitDto>();
            foreach (var item in topList)
            {
                var name = subs.TryGetValue(item.Id, out var code) ? code : "Bilinmeyen";
                result.Add(new TopUnitDto { UnitName = name, Count = item.Count });
            }
            if (other > 0)
                result.Add(new TopUnitDto { UnitName = "Diğer", Count = other });
            return result;
        }

        public async Task<IEnumerable<TopUnitDto>> GetLeastEntrySubAdministrationUnitsAsync(IEnumerable<Guid> userIds, int top = 3)
        {
            var idSet = userIds?.ToHashSet() ?? new HashSet<Guid>();
            var entries = (await _entryRepo.GetAllAsync())
                .Where(e => e.SubAdministrationUnitId.HasValue &&
                            (!idSet.Any() || (e.TenderResponsibleUserId.HasValue && idSet.Contains(e.TenderResponsibleUserId.Value))))
                .ToList();

            var counts = entries
                .GroupBy(e => e.SubAdministrationUnitId!.Value)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .OrderBy(x => x.Count)
                .ToList();

            var topList = counts.Take(top).ToList();
            var other = counts.Skip(top).Sum(x => x.Count);
            var subs = (await _subRepo.GetAllAsync()).ToDictionary(s => s.Id, s => s.Code);
            var result = new List<TopUnitDto>();
            foreach (var item in topList)
            {
                var name = subs.TryGetValue(item.Id, out var code) ? code : "Bilinmeyen";
                result.Add(new TopUnitDto { UnitName = name, Count = item.Count });
            }
            if (other > 0)
                result.Add(new TopUnitDto { UnitName = "Diğer", Count = other });
            return result;
        }

        private static int PeriodDays(string periodType)
        {
            return periodType?.ToLowerInvariant() switch
            {
                "weekly" => 7,
                "monthly" => 30,
                "quarterly" => 90,
                _ => 365,
            };
        }

        public async Task<IEnumerable<UnitPriceStatDto>> GetSubAdministrationAveragePricesAsync(string periodType)
        {
            var days = PeriodDays(periodType);
            var end = DateTime.UtcNow.Date;
            var start = end.AddDays(-days + 1);

            var normals = (await _inspectionRepo.GetAllAsync())
                .Where(c => c.InvoiceDate.Date >= start && c.InvoiceDate.Date <= end)
                .Select(c => new { c.SubAdministrationUnitId, Total = SumCertificate(c) });
            var additionals = (await _addInspectionRepo.GetAllAsync())
                .Where(c => c.InvoiceDate.Date >= start && c.InvoiceDate.Date <= end)
                .Select(c => new { c.SubAdministrationUnitId, Total = SumCertificate(c) });

            var records = normals.Concat(additionals).ToList();
            if (!records.Any())
                return new List<UnitPriceStatDto>();

            var groups = records
                .GroupBy(r => r.SubAdministrationUnitId)
                .Select(g => new { Id = g.Key, Avg = g.Average(x => x.Total) })
                .OrderByDescending(x => x.Avg)
                .ToList();

            var subs = (await _subRepo.GetAllAsync()).ToDictionary(s => s.Id, s => s.Code);
            return groups.Select(g => new UnitPriceStatDto
            {
                UnitName = subs.TryGetValue(g.Id, out var code) ? code : "Bilinmeyen",
                Value = g.Avg
            }).ToList();
        }

        public async Task<IEnumerable<UnitPriceStatDto>> GetThreeSubAdministrationAveragePricesAsync(string periodType)
        {
            var days = PeriodDays(periodType);
            var end = DateTime.UtcNow.Date;
            var start = end.AddDays(-days + 1);

            var normals = (await _inspectionRepo.GetAllAsync())
                .Where(c => c.InvoiceDate.Date >= start && c.InvoiceDate.Date <= end)
                .Select(c => new { c.ThreeSubAdministrationUnitId, Total = SumCertificate(c) });
            var additionals = (await _addInspectionRepo.GetAllAsync())
                .Where(c => c.InvoiceDate.Date >= start && c.InvoiceDate.Date <= end)
                .Select(c => new { c.ThreeSubAdministrationUnitId, Total = SumCertificate(c) });

            var records = normals.Concat(additionals).ToList();
            if (!records.Any())
                return new List<UnitPriceStatDto>();

            var groups = records
                .GroupBy(r => r.ThreeSubAdministrationUnitId)
                .Select(g => new { Id = g.Key, Avg = g.Average(x => x.Total) })
                .OrderByDescending(x => x.Avg)
                .ToList();

            var units = (await _threeSubRepo.GetAllAsync()).ToDictionary(s => s.Id, s => s.Code);
            return groups.Select(g => new UnitPriceStatDto
            {
                UnitName = units.TryGetValue(g.Id, out var code) ? code : "Bilinmeyen",
                Value = g.Avg
            }).ToList();
        }

        public async Task<IEnumerable<TopUnitDto>> GetSubAdministrationCertificateCountsAsync()
        {
            var normals = await _inspectionRepo.GetAllAsync();
            var additionals = await _addInspectionRepo.GetAllAsync();
            var records = normals
                .Select(c => c.SubAdministrationUnitId)
                .Concat(additionals.Select(c => c.SubAdministrationUnitId))
                .GroupBy(id => id)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            var subs = (await _subRepo.GetAllAsync()).ToDictionary(s => s.Id, s => s.Code);
            return records.Select(r => new TopUnitDto
            {
                UnitName = subs.TryGetValue(r.Id, out var code) ? code : "Bilinmeyen",
                Count = r.Count
            }).ToList();
        }

        public async Task<IEnumerable<TopUnitDto>> GetThreeSubAdministrationCertificateCountsAsync()
        {
            var normals = await _inspectionRepo.GetAllAsync();
            var additionals = await _addInspectionRepo.GetAllAsync();
            var records = normals
                .Select(c => c.ThreeSubAdministrationUnitId)
                .Concat(additionals.Select(c => c.ThreeSubAdministrationUnitId))
                .GroupBy(id => id)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            var units = (await _threeSubRepo.GetAllAsync()).ToDictionary(s => s.Id, s => s.Code);
            return records.Select(r => new TopUnitDto
            {
                UnitName = units.TryGetValue(r.Id, out var code) ? code : "Bilinmeyen",
                Count = r.Count
            }).ToList();
        }

        public async Task<IEnumerable<TopUnitDto>> GetSubAdministrationOfferCountsAsync()
        {
            var entries = await _entryRepo.GetAllAsync();
            var subs = entries.Where(e => e.SubAdministrationUnitId.HasValue)
                .ToDictionary(e => e.Id, e => e.SubAdministrationUnitId!.Value);

            var offers = await _offerRepo.GetAllAsync();
            var counts = offers
                .Where(o => subs.ContainsKey(o.ProcurementEntryId))
                .GroupBy(o => subs[o.ProcurementEntryId])
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            var lookup = (await _subRepo.GetAllAsync()).ToDictionary(s => s.Id, s => s.Code);
            return counts.Select(c => new TopUnitDto
            {
                UnitName = lookup.TryGetValue(c.Id, out var code) ? code : "Bilinmeyen",
                Count = c.Count
            }).ToList();
        }

        public async Task<IEnumerable<TopUnitDto>> GetThreeSubAdministrationOfferCountsAsync()
        {
            var entries = await _entryRepo.GetAllAsync();
            var lookupEntry = entries.Where(e => e.ThreeSubAdministrationUnitId.HasValue)
                .ToDictionary(e => e.Id, e => e.ThreeSubAdministrationUnitId!.Value);

            var offers = await _offerRepo.GetAllAsync();
            var counts = offers
                .Where(o => lookupEntry.ContainsKey(o.ProcurementEntryId))
                .GroupBy(o => lookupEntry[o.ProcurementEntryId])
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            var units = (await _threeSubRepo.GetAllAsync()).ToDictionary(s => s.Id, s => s.Code);
            return counts.Select(c => new TopUnitDto
            {
                UnitName = units.TryGetValue(c.Id, out var code) ? code : "Bilinmeyen",
                Count = c.Count
            }).ToList();
        }

        public async Task<IEnumerable<UnitPriceStatDto>> GetSubAdministrationOfferTotalsAsync()
        {
            var entries = await _entryRepo.GetAllAsync();
            var subs = entries.Where(e => e.SubAdministrationUnitId.HasValue)
                .ToDictionary(e => e.Id, e => e.SubAdministrationUnitId!.Value);
            var offers = await _offerRepo.GetAllAsync();
            var totals = offers
                .Where(o => subs.ContainsKey(o.ProcurementEntryId))
                .GroupBy(o => subs[o.ProcurementEntryId])
                .Select(g => new { Id = g.Key, Total = g.Sum(o => o.OfferItems.Sum(i => i.TotalAmount)) })
                .OrderByDescending(x => x.Total)
                .ToList();

            var lookup = (await _subRepo.GetAllAsync()).ToDictionary(s => s.Id, s => s.Code);
            return totals.Select(t => new UnitPriceStatDto
            {
                UnitName = lookup.TryGetValue(t.Id, out var code) ? code : "Bilinmeyen",
                Value = t.Total
            }).ToList();
        }

        public async Task<IEnumerable<UnitPriceStatDto>> GetThreeSubAdministrationOfferTotalsAsync()
        {
            var entries = await _entryRepo.GetAllAsync();
            var lookupEntry = entries.Where(e => e.ThreeSubAdministrationUnitId.HasValue)
                .ToDictionary(e => e.Id, e => e.ThreeSubAdministrationUnitId!.Value);
            var offers = await _offerRepo.GetAllAsync();
            var totals = offers
                .Where(o => lookupEntry.ContainsKey(o.ProcurementEntryId))
                .GroupBy(o => lookupEntry[o.ProcurementEntryId])
                .Select(g => new { Id = g.Key, Total = g.Sum(o => o.OfferItems.Sum(i => i.TotalAmount)) })
                .OrderByDescending(x => x.Total)
                .ToList();

            var units = (await _threeSubRepo.GetAllAsync()).ToDictionary(s => s.Id, s => s.Code);
            return totals.Select(t => new UnitPriceStatDto
            {
                UnitName = units.TryGetValue(t.Id, out var code) ? code : "Bilinmeyen",
                Value = t.Total
            }).ToList();
        }
    }
}
