using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class ProcurementEntryService : IProcurementEntryService
    {
        private readonly MongoDBRepository<ProcurementEntry> _repo;
        private readonly MongoDBRepository<User> _userRepo;
        private readonly IOfferLetterService _offerSvc;
        private readonly IInspectionAcceptanceCertificateService _inspectionSvc;
        private readonly IMapper _mapper;

        public ProcurementEntryService(
            MongoDBRepository<ProcurementEntry> repo,
            MongoDBRepository<User> userRepo,
            IOfferLetterService offerSvc,
            IInspectionAcceptanceCertificateService inspectionSvc,
            IMapper mapper)
        {
            _repo = repo;
            _userRepo = userRepo;
            _offerSvc = offerSvc;
            _inspectionSvc = inspectionSvc;
            _mapper = mapper;
        }

        public async Task<ProcurementEntryDto> CreateAsync(CreateProcurementEntryDto dto)
        {
            var all = await _repo.GetAllAsync();
            if (!string.IsNullOrEmpty(dto.ProcurementDecisionNumber) &&
                all.Any(x => x.ProcurementDecisionNumber.Equals(dto.ProcurementDecisionNumber, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Bu satın alma karar numarası zaten mevcut.");

            var user = await _userRepo.GetByIdAsync(dto.TenderResponsibleUserId);
            if (user == null)
                throw new KeyNotFoundException("İhale sorumlusu user bulunamadı.");

            var entity = _mapper.Map<ProcurementEntry>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<ProcurementEntryDto>(entity);
        }

        public async Task<IEnumerable<ProcurementEntryDto>> GetAllAsync()
        => (await _repo.GetAllAsync()).Select(x => _mapper.Map<ProcurementEntryDto>(x));

        public async Task<ProcurementEntryDto> GetByIdAsync(Guid id)
            => (await _repo.GetByIdAsync(id)) is var e && e != null ? _mapper.Map<ProcurementEntryDto>(e) : null;

        public async Task<ProcurementEntryDto> UpdateAsync(Guid id, UpdateProcurementEntryDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return null;

            // Yeni karar numarası
            var newNumber = dto.ProcurementDecisionNumber ?? string.Empty;

            // Eğer kullanıcı bir karar numarası göndermişse ve bu numara mevcut olandan farklıysa...
            if (!string.IsNullOrWhiteSpace(newNumber)
                && !string.Equals(existing.ProcurementDecisionNumber, newNumber, StringComparison.OrdinalIgnoreCase))
            {
                // ...ve başka bir kayıtta zaten aynı numara varsa
                var all = await _repo.GetAllAsync();
                if (all.Any(x => string.Equals(x.ProcurementDecisionNumber, newNumber, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException("Bu satın alma karar numarası zaten mevcut.");
                }
            }

            var user = await _userRepo.GetByIdAsync(dto.TenderResponsibleUserId);
            if (user == null)
                throw new KeyNotFoundException("İhale sorumlusu user bulunamadı.");

            existing.ProcurementDecisionDate = dto.ProcurementDecisionDate;
            existing.ProcurementDecisionNumber = dto.ProcurementDecisionNumber;
            existing.TenderResponsibleUserId = dto.TenderResponsibleUserId;
            existing.TenderResponsibleTitle = dto.TenderResponsibleTitle;
            existing.WorkName = dto.WorkName;
            existing.WorkReason = dto.WorkReason;
            existing.BudgetAllocationId = dto.BudgetAllocationId;
            existing.SpecificationToBePrepared = dto.SpecificationToBePrepared;
            existing.ContractToBePrepared = dto.ContractToBePrepared;
            existing.PiyasaArastirmaOnayDate = dto.PiyasaArastirmaOnayDate;
            existing.PiyasaArastirmaOnayNumber = dto.PiyasaArastirmaOnayNumber;
            existing.TeklifMektubuDate = dto.TeklifMektubuDate;
            existing.TeklifMektubuNumber = dto.TeklifMektubuNumber;
            existing.PiyasaArastirmaBaslangicDate = dto.PiyasaArastirmaBaslangicDate;
            existing.PiyasaArastirmaBaslangicNumber = dto.PiyasaArastirmaBaslangicNumber;
            existing.YaklasikMaliyetHesaplamaBaslangicDate = dto.YaklasikMaliyetHesaplamaBaslangicDate;
            existing.YaklasikMaliyetHesaplamaBaslangicNumber = dto.YaklasikMaliyetHesaplamaBaslangicNumber;
            existing.MuayeneVeKabulBelgesiDate = dto.MuayeneVeKabulBelgesiDate;
            existing.MuayeneVeKabulBelgesiNumber = dto.MuayeneVeKabulBelgesiNumber;
            existing.AdministrationUnitId = dto.AdministrationUnitId;
            existing.SubAdministrationUnitId = dto.SubAdministrationUnitId;
            existing.ThreeSubAdministrationUnitId = dto.ThreeSubAdministrationUnitId;

            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<ProcurementEntryDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Silinmek istenen Temin Girişi bulunamadı.");

            await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<ProcurementEntryInspectionPriceDto>> GetInspectionPriceRangeAsync(ProcurementEntryInspectionPriceDto query)
        {
            var allEntries = await _repo.GetAllAsync();
            var allInspections = await _inspectionSvc.GetAllAsync();
            var inspectedEntryIds = allInspections
                .Select(i => i.ProcurementEntryId)
                .Distinct()
                .ToHashSet();

            var allOffers = await _offerSvc.GetAllAsync();
            var offersForInspected = allOffers
                .Where(o => inspectedEntryIds.Contains(o.ProcurementEntryId))
                .Select(o => new
                {
                    o.ProcurementEntryId,
                    TotalPrice = (o.OfferItems != null && o.OfferItems.Any())
                        ? o.OfferItems.Sum(fi => fi.TotalAmount)
                        : 0.0
                });

            var priceRanges = offersForInspected
                .GroupBy(x => x.ProcurementEntryId)
                .Select(g => new
                {
                    ProcurementEntryId = g.Key,
                    PriceMin = g.Min(x => x.TotalPrice),
                    PriceMax = g.Max(x => x.TotalPrice)
                })
                .ToList();

            var filtered = priceRanges.Where(p =>
            {
                if (!query.MinPrice.HasValue && !query.MaxPrice.HasValue)
                    return true;

                if (query.MinPrice.HasValue && query.MaxPrice.HasValue)
                    return p.PriceMin >= query.MinPrice.Value && p.PriceMin <= query.MaxPrice.Value;

                if (query.MinPrice.HasValue)
                    return p.PriceMin >= query.MinPrice.Value;

                return query.MaxPrice.HasValue && p.PriceMin <= query.MaxPrice.Value;
            });

            var result = filtered
                .Select(p => new ProcurementEntryInspectionPriceDto
                {
                    ProcurementEntryId = p.ProcurementEntryId,
                    PriceMin = p.PriceMin,
                    PriceMax = p.PriceMax
                })
                .ToList();

            return result;
        }

        public async Task<IEnumerable<ProcurementEntryWithOfferCountDto>> GetByOfferCountAsync(ProcurementEntryWithOfferCountDto query)
        {
            var allEntries = await GetAllAsync();
            var allOffers = await _offerSvc.GetAllAsync();

            var entryToEntreprisePairs = allOffers.Select(o => new
            {
                o.ProcurementEntryId,
                o.EntrepriseId
            });

            var countsPerEntry = entryToEntreprisePairs
                .Distinct()
                .GroupBy(x => x.ProcurementEntryId)
                .Select(g => new
                {
                    ProcurementEntryId = g.Key,
                    OfferCount = g.Count()
                })
                .ToList();

            var filtered = countsPerEntry.Where(p =>
            {
                if (!query.MinCount.HasValue && !query.MaxCount.HasValue)
                    return true;

                if (query.MinCount.HasValue && query.MaxCount.HasValue)
                    return p.OfferCount >= query.MinCount.Value && p.OfferCount <= query.MaxCount.Value;

                if (query.MinCount.HasValue)
                    return p.OfferCount >= query.MinCount.Value;

                return query.MaxCount.HasValue && p.OfferCount <= query.MaxCount.Value;
            });

            var result = filtered
                .Join(
                    allEntries,
                    info => info.ProcurementEntryId,
                    entryDto => entryDto.Id,
                    (info, entryDto) => new ProcurementEntryWithOfferCountDto
                    {
                        ProcurementEntryId = entryDto.Id,
                        WorkName = entryDto.WorkName,
                        OfferCount = info.OfferCount
                    })
                .ToList();

            return result;
        }

        public async Task<IEnumerable<ProcurementEntryWithUnitFilterDto>> GetByAdministrativeUnitsAsync(ProcurementEntryWithUnitFilterDto query)
        {
            var allEntries = await GetAllAsync();

            var filtered = allEntries.Where(entry =>
            {
                if (query.AdministrationUnitId.HasValue &&
                    entry.AdministrationUnitId != query.AdministrationUnitId.Value)
                    return false;

                if (query.SubAdministrationUnitId.HasValue &&
                    entry.SubAdministrationUnitId != query.SubAdministrationUnitId.Value)
                    return false;

                if (query.ThreeSubAdministrationUnitId.HasValue &&
                    entry.ThreeSubAdministrationUnitId != query.ThreeSubAdministrationUnitId.Value)
                    return false;

                return true;
            });

            var result = filtered.Select(entry => new ProcurementEntryWithUnitFilterDto
            {
                ProcurementEntryId = entry.Id,
                WorkName = entry.WorkName,
                AdministrationUnitId = entry.AdministrationUnitId,
                SubAdministrationUnitId = entry.SubAdministrationUnitId,
                ThreeSubAdministrationUnitId = entry.ThreeSubAdministrationUnitId
            }).ToList();

            return result;
        }

        public async Task<IEnumerable<ProcurementEntryDto>> GetByVknAsync(string vkn)
        {
            var allOffers = await _offerSvc.GetAllAsync();

            var matchingEntryIds = allOffers
                .Where(o => o.Vkn.Equals(vkn, StringComparison.OrdinalIgnoreCase))
                .Select(o => o.ProcurementEntryId)
                .Distinct()
                .ToList();

            var resultEntries = new List<ProcurementEntryDto>();
            foreach (var entryId in matchingEntryIds)
            {
                var entry = await GetByIdAsync(entryId);
                if (entry != null)
                    resultEntries.Add(entry);
            }

            return resultEntries;
        }

        public async Task<IEnumerable<ProcurementEntryDto>> GetByInspectionAcceptanceAsync()
        {
            var allEntries = await _repo.GetAllAsync();
            var allInspections = await _inspectionSvc.GetAllAsync();

            var entryIdsWithInspection = allInspections
                .Select(i => i.ProcurementEntryId)
                .Distinct()
                .ToHashSet();

            var filteredEntries = allEntries
                .Where(e => entryIdsWithInspection.Contains(e.Id))
                .Select(e => _mapper.Map<ProcurementEntryDto>(e))
                .ToList();

            return filteredEntries;
        }
    }
}
