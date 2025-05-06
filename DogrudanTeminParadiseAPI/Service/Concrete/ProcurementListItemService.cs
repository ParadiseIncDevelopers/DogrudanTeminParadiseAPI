using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class ProcurementListItemService : IProcurementListItemService
    {
        private readonly MongoDBRepository<ProcurementListItem> _repo;
        private readonly MongoDBRepository<ProcurementEntry> _entryRepo;
        private readonly MongoDBRepository<Unit> _unitRepo;
        private readonly IMapper _mapper;

        public ProcurementListItemService(
            MongoDBRepository<ProcurementListItem> repo,
            MongoDBRepository<ProcurementEntry> entryRepo,
            MongoDBRepository<Unit> unitRepo,
            IMapper mapper)
        {
            _repo = repo;
            _entryRepo = entryRepo;
            _unitRepo = unitRepo;
            _mapper = mapper;
        }

        public async Task<ProcurementListItemDto> CreateAsync(CreateProcurementListItemDto dto)
        {
            // Entry kontrolü
            var entry = await _entryRepo.GetByIdAsync(dto.ProcurementEntryId);
            if (entry == null)
                throw new KeyNotFoundException("Procurement entry bulunamadı.");

            // Unit kontrolü
            var unit = await _unitRepo.GetByIdAsync(dto.UnitId);
            if (unit == null)
                throw new KeyNotFoundException("Unit bulunamadı.");

            // Aynı entry içinde isim duplicate kontrolü
            var all = await _repo.GetAllAsync();
            bool exists = all
                .Where(x => x.ProcurementEntryId == dto.ProcurementEntryId)
                .Any(x => x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase));
            if (exists)
                throw new InvalidOperationException("Bu isimle başka bir kalem mevcut.");

            var entity = _mapper.Map<ProcurementListItem>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<ProcurementListItemDto>(entity);
        }

        public async Task<IEnumerable<ProcurementListItemDto>> GetAllByEntryAsync(Guid procurementEntryId)
        {
            var all = await _repo.GetAllAsync();
            return all
                .Where(x => x.ProcurementEntryId == procurementEntryId)
                .Select(x => _mapper.Map<ProcurementListItemDto>(x));
        }

        public async Task<ProcurementListItemDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<ProcurementListItemDto>(e);
        }

        public async Task DeleteAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null)
                throw new KeyNotFoundException("Kalem bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }

}
