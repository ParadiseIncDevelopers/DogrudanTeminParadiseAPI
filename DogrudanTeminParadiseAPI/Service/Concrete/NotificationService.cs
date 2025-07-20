using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;
using MongoDB.Driver;

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

        public async Task DeleteAllAsync(Guid userId)
        {
            var filter = Builders<Notification>.Filter.Eq(n => n.UserId, userId);
            var list = await _repo.GetAllAsync(filter);
            foreach (var item in list)
            {
                await _repo.DeleteAsync(item.Id);
            }
        }

        public async Task MarkAllIsReadAsync(Guid userId)
        {
            var filter = Builders<Notification>.Filter.Eq(n => n.UserId, userId);
            var list = await _repo.GetAllAsync(filter);
            foreach (var item in list)
            {
                item.IsRead = true;
                await _repo.UpdateAsync(item.Id, item);
            }
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

        public async Task<NotificationDto> MarkIsReadAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Bildirim bulunamadı.");

            existing.IsRead = true;
            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<NotificationDto>(existing);
        }
    }
}
