using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class UserService : IUserService
    {
        private readonly MongoDBRepository<User> _repo;
        private readonly MongoDBRepository<AdminUser> _adminRepo;
        private readonly MongoDBRepository<SuperAdminUser> _sysRepo;
        private readonly MongoDBRepository<Title> _titleRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _cfg;
        private readonly byte[] _aesKey;

        public UserService(
            MongoDBRepository<User> repo,
            MongoDBRepository<AdminUser> adminRepo,
            MongoDBRepository<SuperAdminUser> sysRepo,
            MongoDBRepository<Title> titleRepo,
            IMapper mapper,
            IConfiguration cfg)
        {
            _repo = repo;
            _adminRepo = adminRepo;
            _sysRepo = sysRepo;
            _mapper = mapper;
            _titleRepo = titleRepo;
            _cfg = cfg;

            // AES anahtarı appsettings.json'dan okunur (32 karakter)
            var keyString = _cfg["EncryptionKey"];
            if (string.IsNullOrEmpty(keyString) || keyString.Length != 32)
                throw new InvalidOperationException("Geçersiz AES anahtarı (32 karakter olmalı).");
            _aesKey = Encoding.UTF8.GetBytes(keyString);
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto, string adminId)
        {
            var allUsers = await _repo.GetAllAsync();
            var allAdmins = await _adminRepo.GetAllAsync();
            var getAdmin = await _adminRepo.GetByIdAsync(Guid.Parse(adminId)) ?? throw new InvalidOperationException("Bu Admin kullanıcısı yok. Lütfen tekrar deneyin.");

            var superAdmins = (await _sysRepo.GetAllAsync()).ToList();
            var permissionsDict = superAdmins[0].AssignPermissionToAdmin;

            // Encrypt ve hash
            var encTcid = Crypto.Encrypt(dto.Tcid);
            var encEmail = Crypto.Encrypt(dto.Email);
            var encName = Crypto.Encrypt(dto.Name);
            var encSurname = Crypto.Encrypt(dto.Surname);
            var hashedPwd = Crypto.HashSha512(dto.Password);

            // Uniqueness check
            if (allUsers.Any(u => u.Tcid == encTcid) || allAdmins.Any(u => u.Tcid == encTcid))
                throw new InvalidOperationException("Bu TC Kimlik Numarası zaten kayıtlı.");
            if (allUsers.Any(u => u.Email == encEmail) || allAdmins.Any(u => u.Email == encEmail))
                throw new InvalidOperationException("Bu e-posta zaten kayıtlı.");

            // Entity oluştur
            var entity = _mapper.Map<User>(dto);
            entity.Id = Guid.NewGuid();
            entity.Tcid = encTcid;
            entity.Email = encEmail;
            entity.Name = encName;
            entity.Surname = encSurname;
            entity.Password = hashedPwd;

            if (!permissionsDict.ContainsKey(adminId))
            {
                permissionsDict.Add(adminId, [entity.Id.ToString()]);
            }
            else 
            {
                permissionsDict[adminId].Add(entity.Id.ToString());
            }
            superAdmins[0].AssignPermissionToAdmin = permissionsDict;

            await _repo.InsertAsync(entity);
            await _sysRepo.UpdateAsync(superAdmins[0].Id, superAdmins[0]);

            // Decrypted DTO
            return new UserDto
            {
                Id = entity.Id,
                Name = Crypto.Decrypt(entity.Name),
                Surname = Crypto.Decrypt(entity.Surname),
                Email = Crypto.Decrypt(entity.Email),
                Tcid = Crypto.Decrypt(entity.Tcid),
                UserType = entity.UserType,
                Permissions = entity.Permissions,
                TitleId = entity.TitleId
            };
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(e => new UserDto
            {
                Id = e.Id,
                Name = Crypto.Decrypt(e.Name),
                Surname = Crypto.Decrypt(e.Surname),
                Email = Crypto.Decrypt(e.Email),
                Tcid = Crypto.Decrypt(e.Tcid),
                UserType = e.UserType,
                Permissions = e.Permissions,
                TitleId = e.TitleId
            });
        }

        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;

            return new UserDto
            {
                Id = e.Id,
                Name = Crypto.Decrypt(e.Name),
                Surname = Crypto.Decrypt(e.Surname),
                Email = Crypto.Decrypt(e.Email),
                Tcid = Crypto.Decrypt(e.Tcid),
                UserType = e.UserType,
                Permissions = e.Permissions,
                TitleId = e.TitleId
            };
        }

        public async Task<UserDto> GetProfileAsync(Guid userId)
            => await GetByIdAsync(userId);

        public async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            // Encrypt ve hash
            existing.Tcid = Crypto.Encrypt(dto.Tcid);
            existing.Name = Crypto.Encrypt(dto.Name);
            existing.Surname = Crypto.Encrypt(dto.Surname);
            existing.Email = Crypto.Encrypt(dto.Email);
            existing.Password = Crypto.HashSha512(dto.Password);
            existing.Permissions = dto.Permissions;
            existing.TitleId = dto.TitleId;

            await _repo.UpdateAsync(id, existing);

            return new UserDto
            {
                Id = existing.Id,
                Name = Crypto.Decrypt(existing.Name),
                Surname = Crypto.Decrypt(existing.Surname),
                Email = Crypto.Decrypt(existing.Email),
                Tcid = Crypto.Decrypt(existing.Tcid),
                UserType = existing.UserType,
                Permissions = existing.Permissions,
                TitleId = existing.TitleId
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return;
            await _repo.DeleteAsync(id);
        }

        public async Task ChangePasswordAsync(Guid userId, UpdateUserPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                throw new InvalidOperationException("Yeni parola ve onay eşleşmiyor.");

            var user = await _repo.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");

            var hashedCurrent = Crypto.HashSha512(dto.CurrentPassword);
            if (user.Password != hashedCurrent)
                throw new UnauthorizedAccessException("Mevcut parola yanlış.");

            user.Password = Crypto.HashSha512(dto.NewPassword);
            await _repo.UpdateAsync(userId, user);
        }

        public async Task AssignTitleAsync(Guid userId, Guid titleId)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");

            var title = await _titleRepo.GetByIdAsync(titleId);
            if (title == null)
                throw new KeyNotFoundException("Ünvan bulunamadı.");

            user.TitleId = titleId;
            await _repo.UpdateAsync(userId, user);
        }
    }
}
