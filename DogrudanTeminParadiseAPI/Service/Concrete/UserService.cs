using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class UserService : IUserService
    {
        private readonly MongoDBRepository<User> _repo;
        private readonly IMapper _mapper;

        public UserService(MongoDBRepository<User> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            var allUsers = await _repo.GetAllAsync();

            // Email ve Tcid çakışma kontrolü
            bool emailExists = allUsers.Any(u => u.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase));
            bool tcidExists = allUsers.Any(u => u.Tcid == dto.Tcid);

            if (emailExists)
                throw new InvalidOperationException("Bu e-posta adresi başka bir kullanıcı tarafından kullanılıyor.");
            if (tcidExists)
                throw new InvalidOperationException("Bu TC Kimlik Numarası başka bir kullanıcı tarafından kullanılıyor.");

            var entity = _mapper.Map<User>(dto);
            entity.Id = Guid.NewGuid();

            await _repo.InsertAsync(entity);
            return _mapper.Map<UserDto>(entity);
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var all = await _repo.GetAllAsync();
            return all.Select(u => _mapper.Map<UserDto>(u));
        }

        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            var u = await _repo.GetByIdAsync(id);
            return u == null ? null : _mapper.Map<UserDto>(u);
        }

        public async Task<UserDto> GetProfileAsync(Guid userId)
        {
            return await GetByIdAsync(userId);
        }

        public async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return null;

            existing.Name = dto.Name;
            existing.Surname = dto.Surname;
            existing.Email = dto.Email;
            existing.Password = dto.Password;
            existing.Permissions = dto.Permissions;
            existing.TitleId = dto.TitleId;

            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<UserDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return;
            await _repo.DeleteAsync(id);
        }

        public async Task<User> AuthenticateAsync(string tcid, string password)
        {
            var users = await _repo.GetAllAsync();
            return users.FirstOrDefault(u => u.Tcid == tcid && u.Password == password);
        }

        public async Task<UserDto> AssignTitleAsync(Guid userId, Guid titleId)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null) throw new KeyNotFoundException("Kullanıcı bulunamadı.");

            var title = await _repo.GetByIdAsync(titleId);
            if (title == null) throw new KeyNotFoundException("Ünvan bulunamadı.");

            user.TitleId = titleId;
            await _repo.UpdateAsync(userId, user);
            return _mapper.Map<UserDto>(user);
        }
    }
}
