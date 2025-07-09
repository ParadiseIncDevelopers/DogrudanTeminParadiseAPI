using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class BackupUserNotificationService : IBackupUserNotificationService
    {
        private readonly MongoDBRepository<BackupUserNotification> _repo;
        private readonly IMapper _mapper;

        public BackupUserNotificationService(MongoDBRepository<BackupUserNotification> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<BackupUserNotificationDto> CreateAsync(CreateBackupUserNotificationDto dto)
        {
            var entity = _mapper.Map<BackupUserNotification>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<BackupUserNotificationDto>(entity);
        }

        public async Task<IEnumerable<BackupUserNotificationDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(n => _mapper.Map<BackupUserNotificationDto>(n));
        }

        public async Task<BackupUserNotificationDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<BackupUserNotificationDto>(e);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Kayıt bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}
