using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class NotificationService : INotificationService
    {
        private readonly MongoDBRepository<Notification> _repo;
        private readonly IMapper _mapper;

        public NotificationService(MongoDBRepository<Notification> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<NotificationDto> CreateAsync(CreateNotificationDto dto)
        {
            var entity = _mapper.Map<Notification>(dto);
            entity.Id = Guid.NewGuid();
            entity.CreatedOn = DateTime.UtcNow;
            await _repo.InsertAsync(entity);
            return _mapper.Map<NotificationDto>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Bildirim bulunamadı.");
            await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<NotificationDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(n => _mapper.Map<NotificationDto>(n));
        }

        public async Task<IEnumerable<NotificationDto>> GetAllByUserIdAsync(Guid userId)
        {
            var list = await _repo.GetAllAsync();
            return list.Where(n => n.UserId == userId).Select(n => _mapper.Map<NotificationDto>(n));
        }

        public async Task<NotificationDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<NotificationDto>(entity);
        }
    }
}
