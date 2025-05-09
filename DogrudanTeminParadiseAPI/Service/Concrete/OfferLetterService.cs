using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class OfferLetterService : IOfferLetterService
    {
        private readonly MongoDBRepository<OfferLetter> _repo;
        private readonly MongoDBRepository<Entreprise> _entRepo;
        private readonly MongoDBRepository<ProcurementEntry> _peRepo;
        private readonly MongoDBRepository<Unit> _unitRepo;
        private readonly IMapper _mapper;

        public OfferLetterService(
            MongoDBRepository<OfferLetter> repo,
            MongoDBRepository<Entreprise> entRepo,
            MongoDBRepository<ProcurementEntry> peRepo,
            MongoDBRepository<Unit> unitRepo,
            IMapper mapper)
        {
            _repo = repo;
            _entRepo = entRepo;
            _peRepo = peRepo;
            _unitRepo = unitRepo;
            _mapper = mapper;
        }

        public async Task<OfferLetterDto> CreateAsync(CreateOfferLetterDto dto)
        {
            // ProcurementEntry var mı?
            var entry = await _peRepo.GetByIdAsync(dto.ProcurementEntryId)
                ?? throw new KeyNotFoundException("Procurement entry bulunamadı.");
            // Entreprise var mı?
            var entreprise = await _entRepo.GetByIdAsync(dto.EntrepriseId)
                ?? throw new KeyNotFoundException("Entreprise bulunamadı.");

            // Aynı firma için bu entry'de zaten teklif mektubu var mı?
            bool firmaExists = (await _repo.GetAllAsync())
                .Any(o =>
                    o.ProcurementEntryId == dto.ProcurementEntryId &&
                    o.EntrepriseId == dto.EntrepriseId);
            if (firmaExists)
                throw new InvalidOperationException("Bu firma için zaten bir teklif mektubu oluşturulmuş.");

            // Birim validasyonu
            foreach (var item in dto.OfferItems)
            {
                if (await _unitRepo.GetByIdAsync(item.UnitId) == null)
                    throw new KeyNotFoundException($"Unit {item.UnitId} bulunamadı.");
            }

            // Entity'yi map et ve OfferItem listesini çevir
            var entity = _mapper.Map<OfferLetter>(dto);
            entity.Id = Guid.NewGuid();
            entity.OfferItems = _mapper.Map<List<OfferItem>>(dto.OfferItems);
            entity.Title = dto.Title;
            entity.ResponsiblePerson = dto.ResponsiblePerson;
            entity.Vkn = dto.Vkn;
            entity.NotificationAddress = dto.NotificationAddress;
            entity.Email = dto.Email;
            entity.Nationality = dto.Nationality;

            await _repo.InsertAsync(entity);
            return _mapper.Map<OfferLetterDto>(entity);
        }

        public async Task<IEnumerable<OfferLetterDto>> GetAllByEntryAsync(Guid procurementEntryId)
        {
            var list = (await _repo.GetAllAsync())
                .Where(o => o.ProcurementEntryId == procurementEntryId);
            return list.Select(o => _mapper.Map<OfferLetterDto>(o));
        }

        public async Task<OfferLetterDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<OfferLetterDto>(e);
        }

        public async Task<OfferLetterDto> UpdateAsync(Guid id, UpdateOfferLetterDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return null;

            // Duplicate offer item adı kontrolü
            var otherNames = (await _repo.GetAllAsync())
                .Where(o => o.Id != id && o.ProcurementEntryId == existing.ProcurementEntryId)
                .SelectMany(o => o.OfferItems)
                .Select(i => i.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var item in dto.OfferItems)
            {
                if (otherNames.Contains(item.Name))
                    throw new InvalidOperationException($"OfferItem adı '{item.Name}' zaten mevcut.");
                if (await _unitRepo.GetByIdAsync(item.UnitId) == null)
                    throw new KeyNotFoundException($"Unit {item.UnitId} bulunamadı.");
            }

            existing.OfferItems = dto.OfferItems.Select(i => _mapper.Map<OfferItem>(i)).ToList();
            existing.NotificationAddress = dto.NotificationAddress;
            existing.Nationality = dto.Nationality;
            existing.ResponsiblePerson = dto.ResponsiblePerson;

            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<OfferLetterDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Teklif mektubu bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}
