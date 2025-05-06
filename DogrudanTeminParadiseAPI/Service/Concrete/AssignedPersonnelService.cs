using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class AssignedPersonnelService : IAssignedPersonnelService
    {
        private readonly MongoDBRepository<AssignedPersonnel> _repo;
        private readonly MongoDBRepository<ProcurementEntry> _peRepo;
        private readonly MongoDBRepository<User> _userRepo;
        private readonly IMapper _mapper;

        public AssignedPersonnelService(
            MongoDBRepository<AssignedPersonnel> repo,
            MongoDBRepository<ProcurementEntry> peRepo,
            MongoDBRepository<User> userRepo,
            IMapper mapper)
        {
            _repo = repo;
            _peRepo = peRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<AssignedPersonnelDto> CreateAsync(CreateAssignedPersonnelDto dto)
        {
            // ProcurementEntry kontrolü
            if (await _peRepo.GetByIdAsync(dto.ProcurementEntryId) == null)
                throw new KeyNotFoundException("Temin girişi bulunamadı.");

            // Zaten atanmış mı?
            var exists = (await _repo.GetAllAsync())
                .Any(x => x.ProcurementEntryId == dto.ProcurementEntryId);
            if (exists)
                throw new InvalidOperationException("Bu temin girişine zaten personel atanmış.");

            // Kullanıcı kontrolleri
            if (await _userRepo.GetByIdAsync(dto.UserId1) == null)
                throw new KeyNotFoundException("User1 bulunamadı.");
            if (await _userRepo.GetByIdAsync(dto.UserId2) == null)
                throw new KeyNotFoundException("User2 bulunamadı.");
            if (await _userRepo.GetByIdAsync(dto.UserId3) == null)
                throw new KeyNotFoundException("User3 bulunamadı.");

            var entity = new AssignedPersonnel
            {
                Id = Guid.NewGuid(),
                ProcurementEntryId = dto.ProcurementEntryId,
                UserId1 = dto.UserId1,
                UserId2 = dto.UserId2,
                UserId3 = dto.UserId3
            };
            await _repo.InsertAsync(entity);
            return _mapper.Map<AssignedPersonnelDto>(entity);
        }

        public async Task<AssignedPersonnelDto> UpdateAsync(Guid procurementEntryId, CreateAssignedPersonnelDto dto)
        {
            var existing = (await _repo.GetAllAsync())
                .FirstOrDefault(x => x.ProcurementEntryId == procurementEntryId);
            if (existing == null)
                throw new KeyNotFoundException("Atanmış personel kaydı bulunamadı.");

            // Kullanıcı kontrolleri
            if (await _userRepo.GetByIdAsync(dto.UserId1) == null)
                throw new KeyNotFoundException("User1 bulunamadı.");
            if (await _userRepo.GetByIdAsync(dto.UserId2) == null)
                throw new KeyNotFoundException("User2 bulunamadı.");
            if (await _userRepo.GetByIdAsync(dto.UserId3) == null)
                throw new KeyNotFoundException("User3 bulunamadı.");

            existing.UserId1 = dto.UserId1;
            existing.UserId2 = dto.UserId2;
            existing.UserId3 = dto.UserId3;
            await _repo.UpdateAsync(existing.Id, existing);
            return _mapper.Map<AssignedPersonnelDto>(existing);
        }

        public async Task<AssignedPersonnelDto> GetByEntryAsync(Guid procurementEntryId)
        {
            var existing = (await _repo.GetAllAsync())
                .FirstOrDefault(x => x.ProcurementEntryId == procurementEntryId);
            return existing == null ? null : _mapper.Map<AssignedPersonnelDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Personel kaydı bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}
