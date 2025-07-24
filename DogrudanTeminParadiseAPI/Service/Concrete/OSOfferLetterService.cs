using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class OSOfferLetterService : IOSOfferLetterService
    {
        private readonly MongoDBRepository<OSOfferLetter> _repo;
        private readonly MongoDBRepository<Entreprise> _entRepo;
        private readonly MongoDBRepository<OSProcurementEntry> _entryRepo;
        private readonly IMapper _mapper;

        public OSOfferLetterService(
            MongoDBRepository<OSOfferLetter> repo,
            MongoDBRepository<Entreprise> entRepo,
            MongoDBRepository<OSProcurementEntry> entryRepo,
            IMapper mapper)
        {
            _repo = repo;
            _entRepo = entRepo;
            _entryRepo = entryRepo;
            _mapper = mapper;
        }

        public async Task<OSOfferLetterDto> CreateAsync(CreateOSOfferLetterDto dto)
        {
            var entry = await _entryRepo.GetByIdAsync(dto.OneSourceProcurementEntryId)
                ?? throw new KeyNotFoundException("Entry bulunamadı.");
            var entreprise = await _entRepo.GetByIdAsync(dto.EntrepriseId)
                ?? throw new KeyNotFoundException("Firma bulunamadı.");

            bool exists = (await _repo.GetAllAsync())
                .Any(o => o.OneSourceProcurementEntryId == dto.OneSourceProcurementEntryId
                       && o.EntrepriseId == dto.EntrepriseId);
            if (exists)
                throw new InvalidOperationException("Zaten oluşturulmuş.");

            var entity = _mapper.Map<OSOfferLetter>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<OSOfferLetterDto>(entity);
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<OSOfferLetterDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(_mapper.Map<OSOfferLetterDto>);
        }

        public async Task<IEnumerable<OSOfferLetterDto>> GetAllAsync(IEnumerable<Guid> permittedEntryIds)
        {
            if (permittedEntryIds == null)
                return Enumerable.Empty<OSOfferLetterDto>();
            var list = await _repo.GetAllAsync();
            return list.Where(o => permittedEntryIds.Contains(o.OneSourceProcurementEntryId))
                       .Select(_mapper.Map<OSOfferLetterDto>);
        }

        public async Task<IEnumerable<OSOfferLetterDto>> GetAllByEntryAsync(Guid entryId)
        {
            var list = await _repo.GetAllAsync();
            return list.Where(o => o.OneSourceProcurementEntryId == entryId)
                       .Select(_mapper.Map<OSOfferLetterDto>);
        }

        public async Task<IEnumerable<OSOfferLetterDto>> GetAllByEntryAsync(Guid entryId, IEnumerable<Guid> permittedEntryIds)
        {
            if (permittedEntryIds == null || !permittedEntryIds.Contains(entryId))
                return Enumerable.Empty<OSOfferLetterDto>();
            var list = await _repo.GetAllAsync();
            return list.Where(o => o.OneSourceProcurementEntryId == entryId)
                       .Select(_mapper.Map<OSOfferLetterDto>);
        }

        public async Task<OSOfferLetterDto?> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<OSOfferLetterDto>(e);
        }

        public async Task<OSOfferLetterDto?> UpdateAsync(Guid id, UpdateOSOfferLetterDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;
            _mapper.Map(dto, existing);
            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<OSOfferLetterDto>(existing);
        }

        public async Task<IEnumerable<OSOfferLetterDto>> UpdateItemsByEntryAsync(Guid entryId, UpdateOSOfferItemsByEntryDto dto)
        {
            if (entryId != dto.OneSourceProcurementEntryId)
                throw new ArgumentException("Entry IDs do not match.", nameof(dto));
            var all = await _repo.GetAllAsync();
            var letters = all.Where(o => o.OneSourceProcurementEntryId == entryId).ToList();
            if (!letters.Any())
                throw new KeyNotFoundException("Bu entryId için teklif mektubu bulunamadı.");
            var qtyMap = dto.Items.ToDictionary(i => i.OfferItemId, i => i.Qty);
            foreach (var letter in letters)
            {
                var updated = false;
                foreach (var item in letter.OfferItems)
                {
                    if (qtyMap.TryGetValue(item.Id, out var newQty))
                    {
                        item.Quantity = newQty;
                        updated = true;
                    }
                }
                if (updated)
                    await _repo.UpdateAsync(letter.Id, letter);
            }
            return letters.Select(_mapper.Map<OSOfferLetterDto>);
        }
    }
}
