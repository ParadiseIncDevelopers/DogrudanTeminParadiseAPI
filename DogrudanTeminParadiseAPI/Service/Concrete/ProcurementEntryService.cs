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
        private readonly MongoDBRepository<AdminUser> _adminRepo;
        private readonly IAdditionalInspectionAcceptanceService _additionalInspectionRepo;
        private readonly IOfferLetterService _offerSvc;
        private readonly IInspectionAcceptanceCertificateService _inspectionSvc;
        private readonly IMapper _mapper;

        public ProcurementEntryService(MongoDBRepository<ProcurementEntry> repo, MongoDBRepository<User> userRepo, MongoDBRepository<AdminUser> adminRepo, IOfferLetterService offerSvc, IInspectionAcceptanceCertificateService inspectionSvc,  IMapper mapper, IAdditionalInspectionAcceptanceService additionalInspectionRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
            _adminRepo = adminRepo;
            _offerSvc = offerSvc;
            _inspectionSvc = inspectionSvc;
            _mapper = mapper;
            _additionalInspectionRepo = additionalInspectionRepo;
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

        public async Task<IEnumerable<ProcurementEntryDto>> GetAllAsync(IEnumerable<Guid> permittedEntryIds)
        {
            if (permittedEntryIds == null)
                return [];

            var list = await _repo.GetAllAsync();
            return list
                .Where(e => permittedEntryIds.Contains(e.Id))
                .Select(x => _mapper.Map<ProcurementEntryDto>(x));
        }

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

        public async Task<IEnumerable<ProcurementEntryDto>> GetInspectionPriceRangeAsync(ProcurementEntryInspectionPriceDto query)
        {
            // 1) Tüm temin girişlerini al
            var allEntries = await _repo.GetAllAsync();

            // 2) Tüm muayene kabul ve ek muayene kabul kayıtlarını al
            var inspections = (await _inspectionSvc.GetAllAsync()).ToList();
            var additionals = (await _additionalInspectionRepo.GetAllAsync()).ToList();

            // 3) Her sertifika için toplam fiyatı hesapla (SelectedProducts üzerinden)
            var inspectionTotals = inspections
                .Select(c => new
                {
                    EntryId = c.ProcurementEntryId,
                    Total = c.SelectedProducts.Sum(p => p.UnitPrice * p.Quantity)
                });

            var additionalTotals = additionals
                .Select(c => new
                {
                    EntryId = c.ProcurementEntryId,
                    Total = c.SelectedProducts.Sum(p => p.UnitPrice * p.Quantity)
                });

            // 4) EntryId başına en düşük toplam fiyatı bul
            var combined = inspectionTotals.Concat(additionalTotals);
            var minPricesByEntry = combined
                .GroupBy(x => x.EntryId)
                .Select(g => new
                {
                    EntryId = g.Key,
                    MinTotal = g.Min(x => x.Total)
                })
                .ToDictionary(x => x.EntryId, x => x.MinTotal);

            // 5) Filtre koşulunu uygula
            var filteredEntryIds = new HashSet<Guid>();
            foreach (var kvp in minPricesByEntry)
            {
                var price = kvp.Value;
                // Eğer hem MinPrice hem MaxPrice sağlanmışsa
                if (query.MinOfferPrice.HasValue && query.MaxOfferPrice.HasValue)
                {
                    if (price >= query.MinOfferPrice.Value && price <= query.MaxOfferPrice.Value)
                        filteredEntryIds.Add(kvp.Key);
                }
                else if (query.MinOfferPrice.HasValue) // Sadece MinPrice varsa
                {
                    if (price >= query.MinOfferPrice.Value)
                        filteredEntryIds.Add(kvp.Key);
                }
                else if (query.MaxOfferPrice.HasValue) // Sadece MaxPrice varsa
                {
                    if (price <= query.MaxOfferPrice.Value)
                        filteredEntryIds.Add(kvp.Key);
                }
                else // Hiç filter yoksa, tüm entry’leri al
                {
                    filteredEntryIds.Add(kvp.Key);
                }
            }

            // 6) EntryId’si sertifika barındıran ve filtreye uyanları DTO’ya çevir
            var result = allEntries
                .Where(e => filteredEntryIds.Contains(e.Id))
                .Select(e => _mapper.Map<ProcurementEntryDto>(e))
                .ToList();

            return result;
        }

        public async Task<IEnumerable<ProcurementEntryDto>> GetByOfferCountAsync(ProcurementEntryWithOfferCountDto query)
        {
            var allOffers = await _offerSvc.GetAllAsync();
            var allEntries = await _repo.GetAllAsync();

            // EntryId başına düşen offer sayısını hesapla
            var offerCounts = allOffers
                .GroupBy(o => o.ProcurementEntryId)
                .Select(g => new
                {
                    ProcurementEntryId = g.Key,
                    Count = g.Count()
                })
                .ToDictionary(x => x.ProcurementEntryId, x => x.Count);

            // Filtre koşullarına göre EntryId listesi oluştur
            var filteredEntryIds = offerCounts
                .Where(kvp =>
                {
                    var count = kvp.Value;
                    if (query.MinOfferPrice.HasValue && count < query.MinOfferPrice.Value)
                        return false;
                    if (query.MaxOfferPrice.HasValue && count > query.MaxOfferPrice.Value)
                        return false;
                    return true;
                })
                .Select(kvp => kvp.Key)
                .ToHashSet();

            // Eklenecek: offerCounts’da hiç kayıt yoksa Count = 0 kabul edilebilir. 
            // Eğer MinCount == 0 veya MaxCount >= 0 isteniyorsa, aşağıdaki satırı ekleyerek sıfır offer’lı entry’ler de dahil edilir:
            if (query.MinOfferPrice.HasValue && query.MinOfferPrice.Value == 0)
            {
                var zeroOfferIds = allEntries
                    .Where(e => !offerCounts.ContainsKey(e.Id))
                    .Select(e => e.Id);

                foreach (var id in zeroOfferIds)
                    filteredEntryIds.Add(id);
            }

            // Son olarak eşleşen ProcurementEntry’leri al ve DTO’ya map et
            var result = allEntries
                .Where(e => filteredEntryIds.Contains(e.Id))
                .Select(e => _mapper.Map<ProcurementEntryDto>(e))
                .ToList();

            return result;
        }

        public async Task<IEnumerable<ProcurementEntryDto>> GetByAdministrativeUnitsAsync(ProcurementEntryWithUnitFilterDto query)
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

            var result = filtered
                .Select(e => _mapper.Map<ProcurementEntryDto>(e))
                .ToList();

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

        public async Task<IEnumerable<ProcurementEntryDto>> GetByBudgetAllocationAsync(Guid budgetAllocationId)
        {
            var allEntries = await _repo.GetAllAsync();
            var filtered = allEntries
                .Where(e => e.BudgetAllocationId.HasValue && e.BudgetAllocationId.Value == budgetAllocationId)
                .Select(e => _mapper.Map<ProcurementEntryDto>(e))
                .ToList();
            return filtered;
        }

        public async Task<IEnumerable<ProcurementEntryDto>> GetByInspectionDateRangeAsync(ProcurementEntryDateRangeDto query)
        {
            // Öncelikle tüm girişleri al
            var allEntries = await _repo.GetAllAsync();

            // Tüm muayene kabul ve ek muayene kabul kayıtlarını al
            var inspections = (await _inspectionSvc.GetAllAsync()).ToList();
            var additionals = (await _additionalInspectionRepo.GetAllAsync()).ToList();

            // Tarih filtre validasyonu: Eğer her ikisi de varsa ve EndDate < StartDate ise hata
            if (query.StartDate.HasValue && query.EndDate.HasValue &&
                query.EndDate.Value.Date < query.StartDate.Value.Date)
            {
                throw new ArgumentException("Bitiş tarihi, başlangıç tarihinden önce olamaz.");
            }

            // Belirlenen aralığa göre sertifikaları filtrele
            IEnumerable<Guid> matchingEntryIds = [];

            if (query.StartDate.HasValue || query.EndDate.HasValue)
            {
                var start = query.StartDate?.Date ?? DateTime.MinValue;
                var end = query.EndDate?.Date ?? DateTime.MaxValue;

                // InspectionAcceptance
                var inspIds = inspections
                    .Where(c => c.InvoiceDate.Date >= start && c.InvoiceDate.Date <= end)
                    .Select(c => c.ProcurementEntryId);

                // AdditionalInspectionAcceptance
                var addIds = additionals
                    .Where(c => c.InvoiceDate.Date >= start && c.InvoiceDate.Date <= end)
                    .Select(c => c.ProcurementEntryId);

                matchingEntryIds = inspIds.Concat(addIds).Distinct();
            }
            else
            {
                // Hiç tarih ayarlanmadıysa, tüm girişleri alabilmek için sertifika sahip olan EntryId’ler
                var inspIds = inspections.Select(c => c.ProcurementEntryId);
                var addIds = additionals.Select(c => c.ProcurementEntryId);
                matchingEntryIds = inspIds.Concat(addIds).Distinct();
            }

            // Eşleşen EntryId’leri DTO’ya map et
            var result = allEntries
                .Where(e => matchingEntryIds.Contains(e.Id))
                .Select(e => _mapper.Map<ProcurementEntryDto>(e))
                .ToList();

            return result;
        }

        public async Task<IEnumerable<ProcurementEntryDto>> GetByRequesterAsync(Guid requesterId, bool isAdmin)
        {
            var allEntries = await _repo.GetAllAsync();
            if (isAdmin)
            {
                var allAdminIds = (await _adminRepo.GetAllAsync()).Select(a => a.Id).ToHashSet();
                return allEntries
                    .Where(e =>
                        // include if created by a normal user
                        !allAdminIds.Contains(e.TenderResponsibleUserId.GetValueOrDefault())
                        // or created by this same admin
                        || e.TenderResponsibleUserId == requesterId
                    )
                    .Select(e => _mapper.Map<ProcurementEntryDto>(e));
            }
            else
            {
                return allEntries
                    .Where(e => e.TenderResponsibleUserId == requesterId)
                    .Select(e => _mapper.Map<ProcurementEntryDto>(e));
            }
        }
    }
}
