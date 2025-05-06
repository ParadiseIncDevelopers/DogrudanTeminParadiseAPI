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
            var entry = await _peRepo.GetByIdAsync(dto.ProcurementEntryId)
                ?? throw new KeyNotFoundException("Procurement entry bulunamadı.");
            var ent = await _entRepo.GetByIdAsync(dto.EntrepriseId)
                ?? throw new KeyNotFoundException("Entreprise bulunamadı.");

            // İsim uniqueness kontrolü
            var existing = (await _repo.GetAllAsync())
                .Where(o => o.ProcurementEntryId == dto.ProcurementEntryId)
                .SelectMany(o => o.OfferItems)
                .Select(i => i.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var item in dto.OfferItems)
            {
                if (existing.Contains(item.Name))
                    throw new InvalidOperationException($"OfferItem adı '{item.Name}' zaten mevcut.");
            }

            // Birim validasyon
            foreach (var item in dto.OfferItems)
            {
                if (await _unitRepo.GetByIdAsync(item.UnitId) == null)
                    throw new KeyNotFoundException($"Unit {item.UnitId} bulunamadı.");
            }

            var entity = _mapper.Map<OfferLetter>(dto);
            entity.Id = Guid.NewGuid();
            entity.Title = ent.Unvan;
            entity.ResponsiblePerson = ent.FirmaYetkilisi;
            entity.Vkn = ent.Vkn;
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

            // Duplicate check
            var allItems = (await _repo.GetAllAsync())
                .Where(o => o.Id != id && o.ProcurementEntryId == existing.ProcurementEntryId)
                .SelectMany(o => o.OfferItems)
                .Select(i => i.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var item in dto.OfferItems)
            {
                if (allItems.Contains(item.Name))
                    throw new InvalidOperationException($"OfferItem adı '{item.Name}' zaten mevcut.");
                if (await _unitRepo.GetByIdAsync(item.UnitId) == null)
                    throw new KeyNotFoundException($"Unit {item.UnitId} bulunamadı.");
            }

            existing.OfferItems = dto.OfferItems.Select(i => _mapper.Map<OfferItem>(i)).ToList();
            existing.NotificationAddress = dto.NotificationAddress;
            existing.Email = dto.Email;
            existing.Nationality = dto.Nationality;

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
