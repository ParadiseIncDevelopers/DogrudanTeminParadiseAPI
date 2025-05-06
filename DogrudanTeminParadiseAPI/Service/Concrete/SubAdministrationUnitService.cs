using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class SubAdministrationUnitService : ISubAdministrationUnitService
    {
        private readonly MongoDBRepository<SubAdministrationUnit> _repo;
        private readonly MongoDBRepository<AdministrationUnit> _parentRepo;
        private readonly IMapper _mapper;

        public SubAdministrationUnitService(
            MongoDBRepository<SubAdministrationUnit> repo,
            MongoDBRepository<AdministrationUnit> parentRepo,
            IMapper mapper)
        {
            _repo = repo;
            _parentRepo = parentRepo;
            _mapper = mapper;
        }

        public async Task<SubAdministrationUnitDto> CreateAsync(CreateSubAdministrationUnitDto dto)
        {
            // 1) Üst birim var mı?
            var parent = await _parentRepo.GetByIdAsync(dto.AdministrationUnitId);
            if (parent == null)
                throw new KeyNotFoundException("Belirtilen İdare Birimi bulunamadı.");

            var all = await _repo.GetAllAsync();

            // 2) Aynı üst birim altında duplicate isim kontrolü
            bool nameExists = all
                .Where(x => x.AdministrationUnitId == dto.AdministrationUnitId)
                .Any(x => x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase));

            if (nameExists)
                throw new InvalidOperationException("Bu üst birim altında aynı isimde bir alt birim zaten mevcut.");

            var entity = _mapper.Map<SubAdministrationUnit>(dto);
            entity.Id = Guid.NewGuid();

            await _repo.InsertAsync(entity);
            return _mapper.Map<SubAdministrationUnitDto>(entity);
        }

        public async Task<IEnumerable<SubAdministrationUnitDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(x => _mapper.Map<SubAdministrationUnitDto>(x));
        }

        public async Task<SubAdministrationUnitDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<SubAdministrationUnitDto>(e);
        }

        public async Task<SubAdministrationUnitDto> UpdateAsync(Guid id, UpdateSubAdministrationUnitDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return null;

            // Duplicate kontrol: eğer isim değiştiyse
            if (!existing.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                var all = await _repo.GetAllAsync();
                bool nameExists = all
                    .Where(x => x.AdministrationUnitId == existing.AdministrationUnitId)
                    .Any(x => x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase));

                if (nameExists)
                    throw new InvalidOperationException("Bu üst birim altında aynı isimde bir alt birim zaten mevcut.");
            }

            existing.Name = dto.Name;
            existing.Code = dto.Code;
            existing.Description = dto.Description;

            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<SubAdministrationUnitDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Silinmek istenen alt birim bulunamadı.");

            await _repo.DeleteAsync(id);
        }
    }
}
