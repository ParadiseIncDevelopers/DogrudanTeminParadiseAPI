using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class UserNotificationService : IUserNotificationService
    {
        private readonly MongoDBRepository<UserNotification> _repo;
        private readonly MongoDBRepository<BackupUserNotification> _backupRepo;
        private readonly IMapper _mapper;

        public UserNotificationService(
            MongoDBRepository<UserNotification> repo,
            MongoDBRepository<BackupUserNotification> backupRepo,
            IMapper mapper)
        {
            _repo = repo;
            _backupRepo = backupRepo;
            _mapper = mapper;
        }

        public async Task<UserNotificationDto> CreateAsync(CreateUserNotificationDto dto)
        {
            var entity = _mapper.Map<UserNotification>(dto);
            entity.Id = Guid.NewGuid();
            entity.NotificationDate = DateTime.UtcNow;
            await _repo.InsertAsync(entity);
            return _mapper.Map<UserNotificationDto>(entity);
        }

        public async Task<IEnumerable<UserNotificationDto>> GetAsync(UserNotificationQueryDto query)
        {
            var list = await _repo.GetAllAsync();
            if (query.FromDate.HasValue)
                list = list.Where(n => n.NotificationDate >= query.FromDate.Value);
            if (query.ToDate.HasValue)
                list = list.Where(n => n.NotificationDate <= query.ToDate.Value);
            if (query.ToUsers != null && query.ToUsers.Any())
                list = list.Where(n => query.ToUsers.Contains(n.NotificationToUser));
            if (!string.IsNullOrWhiteSpace(query.Header))
                list = list.Where(n => (n.NotificationHeader ?? string.Empty).Contains(query.Header, StringComparison.OrdinalIgnoreCase));
            list = list.OrderByDescending(n => n.NotificationDate).Take(query.Top);
            return list.Select(n => _mapper.Map<UserNotificationDto>(n));
        }

        public async Task<UserNotificationDto> UpdateAsync(Guid id, UpdateUserNotificationDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Bildirim bulunamadı.");
            existing.IsMarkAsUnread = dto.IsMarkAsUnread;
            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<UserNotificationDto>(existing);
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Bildirim bulunamadı.");
            var backup = _mapper.Map<BackupUserNotification>(existing);
            backup.Id = Guid.NewGuid();
            backup.RemovedByUserId = userId;
            backup.RemovingDate = DateTime.UtcNow;
            await _backupRepo.InsertAsync(backup);
            await _repo.DeleteAsync(id);
        }
    }
}
