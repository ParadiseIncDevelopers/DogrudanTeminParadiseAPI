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
                ?? throw new KeyNotFoundException("Entry bulunamadı.");

            var entreprise = await _entRepo.GetByIdAsync(dto.EntrepriseId);
            if (entreprise == null)
                throw new KeyNotFoundException("Firma bulunamadı.");

            bool exists = (await _repo.GetAllAsync())
                .Any(o => o.ProcurementEntryId == dto.ProcurementEntryId
                       && o.EntrepriseId == dto.EntrepriseId);
            if (exists)
                throw new InvalidOperationException("Zaten oluşturulmuş.");
            try
            {
                var entity = _mapper.Map<OfferLetter>(dto);
                entity.Id = Guid.NewGuid();
                await _repo.InsertAsync(entity);
                return _mapper.Map<OfferLetterDto>(entity);
            }
            catch (Exception e) 
            {
                return null;
            }
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
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            _mapper.Map(dto, e);
            await _repo.UpdateAsync(id, e);
            return _mapper.Map<OfferLetterDto>(e);
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
