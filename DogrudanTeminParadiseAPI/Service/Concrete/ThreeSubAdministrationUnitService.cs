using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class ThreeSubAdministrationUnitService : IThreeSubAdministrationUnitService
    {
        private readonly MongoDBRepository<ThreeSubAdministrationUnit> _repo;
        private readonly MongoDBRepository<AdministrationUnit> _adminRepo;
        private readonly MongoDBRepository<SubAdministrationUnit> _subAdminRepo;
        private readonly IMapper _mapper;

        public ThreeSubAdministrationUnitService(
            MongoDBRepository<ThreeSubAdministrationUnit> repo,
            MongoDBRepository<AdministrationUnit> adminRepo,
            MongoDBRepository<SubAdministrationUnit> subAdminRepo,
            IMapper mapper)
        {
            _repo = repo;
            _adminRepo = adminRepo;
            _subAdminRepo = subAdminRepo;
            _mapper = mapper;
        }

        public async Task<ThreeSubAdministrationUnitDto> CreateAsync(CreateThreeSubAdministrationUnitDto dto)
        {
            // Parent existence
            var admin = await _adminRepo.GetByIdAsync(dto.AdministrationUnitId);
            if (admin == null)
                throw new KeyNotFoundException("Belirtilen İdare Birimi bulunamadı.");
            var sub = await _subAdminRepo.GetByIdAsync(dto.SubAdministrationUnitId);
            if (sub == null)
                throw new KeyNotFoundException("Belirtilen Alt İdare Birimi bulunamadı.");

            var all = await _repo.GetAllAsync();
            // Duplicate name under same hierarchy
            bool exists = all
                .Where(x => x.AdministrationUnitId == dto.AdministrationUnitId
                         && x.SubAdministrationUnitId == dto.SubAdministrationUnitId)
                .Any(x => x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase));
            if (exists)
                throw new InvalidOperationException("Bu üst ve alt birim altında aynı isimde bir kayıt mevcut.");

            var entity = _mapper.Map<ThreeSubAdministrationUnit>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<ThreeSubAdministrationUnitDto>(entity);
        }

        public async Task<IEnumerable<ThreeSubAdministrationUnitDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(x => _mapper.Map<ThreeSubAdministrationUnitDto>(x));
        }

        public async Task<ThreeSubAdministrationUnitDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<ThreeSubAdministrationUnitDto>(e);
        }

        public async Task<ThreeSubAdministrationUnitDto> UpdateAsync(Guid id, UpdateThreeSubAdministrationUnitDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return null;

            if (!existing.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                var all = await _repo.GetAllAsync();
                bool exists = all
                    .Where(x => x.AdministrationUnitId == existing.AdministrationUnitId
                             && x.SubAdministrationUnitId == existing.SubAdministrationUnitId)
                    .Any(x => x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase));
                if (exists)
                    throw new InvalidOperationException("Bu üst ve alt birim altında aynı isimde bir kayıt mevcut.");
            }

            existing.Name = dto.Name;
            existing.Code = dto.Code;
            existing.Description = dto.Description;

            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<ThreeSubAdministrationUnitDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Silinmek istenen kayıt bulunamadı.");

            await _repo.DeleteAsync(id);
        }
    }
}
