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
    public class AdminUserService : IAdminUserService
    {
        private readonly MongoDBRepository<AdminUser> _repo;
        private readonly MongoDBRepository<User> _userRepo;
        private readonly MongoDBRepository<Title> _titleRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _cfg;

        public AdminUserService(
            MongoDBRepository<AdminUser> repo,
            MongoDBRepository<User> userRepo,
            MongoDBRepository<Title> titleRepo,
            IMapper mapper,
            IConfiguration cfg)
        {
            _repo = repo;
            _userRepo = userRepo;
            _titleRepo = titleRepo;
            _mapper = mapper;
            _cfg = cfg;
        }

        public async Task<AdminUserDto> CreateAsync(CreateUserDto dto)
        {
            var allAdmins = await _repo.GetAllAsync();
            var allUsers = await _userRepo.GetAllAsync();

            // AES ile şifreleyeceğimiz alanlar
            var encTcid = Crypto.Encrypt(dto.Tcid);
            var encEmail = Crypto.Encrypt(dto.Email);
            var encName = Crypto.Encrypt(dto.Name);
            var encSurname = Crypto.Encrypt(dto.Surname);
            var encPublicInstitution =
                string.IsNullOrEmpty(dto.PublicInstitutionName)
                    ? null
                    : Crypto.Encrypt(dto.PublicInstitutionName);

            var hashedPwd = Crypto.HashSha512(dto.Password);

            if (allAdmins.Any(u => u.Tcid == encTcid) || allUsers.Any(u => u.Tcid == encTcid))
                throw new InvalidOperationException("Bu TC Kimlik Numarası zaten sistemde kayıtlı.");
            if (allAdmins.Any(u => u.Email == encEmail) || allUsers.Any(u => u.Email == encEmail))
                throw new InvalidOperationException("Bu e-posta adresi zaten sistemde kayıtlı.");

            var entity = new AdminUser
            {
                Id = Guid.NewGuid(),
                Name = encName,
                Surname = encSurname,
                Email = encEmail,
                Tcid = encTcid,
                Password = hashedPwd,
                UserType = dto.UserType,
                Permissions = dto.Permissions,
                TitleId = dto.TitleId,
                PublicInstitutionName = encPublicInstitution
            };

            await _repo.InsertAsync(entity);

            return new AdminUserDto
            {
                Id = entity.Id,
                Name = Crypto.Decrypt(entity.Name),
                Surname = Crypto.Decrypt(entity.Surname),
                Email = Crypto.Decrypt(entity.Email),
                Tcid = Crypto.Decrypt(entity.Tcid),
                UserType = entity.UserType,
                TitleId = entity.TitleId,
                Permissions = entity.Permissions,
                PublicInstitutionName = entity.PublicInstitutionName == null
                                            ? null
                                            : Crypto.Decrypt(entity.PublicInstitutionName)
            };
        }

        public async Task<AdminUserDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;

            return new AdminUserDto
            {
                Id = e.Id,
                Name = Crypto.Decrypt(e.Name),
                Surname = Crypto.Decrypt(e.Surname),
                Email = Crypto.Decrypt(e.Email),
                Tcid = Crypto.Decrypt(e.Tcid),
                UserType = e.UserType,
                TitleId = e.TitleId,
                Permissions = e.Permissions,
                PublicInstitutionName = string.IsNullOrEmpty(e.PublicInstitutionName)
                                            ? null
                                            : e.PublicInstitutionName
            };
        }

        public async Task<IEnumerable<AdminUserDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(e => new AdminUserDto
            {
                Id = e.Id,
                Name = Crypto.Decrypt(e.Name),
                Surname = Crypto.Decrypt(e.Surname),
                Email = Crypto.Decrypt(e.Email),
                Tcid = Crypto.Decrypt(e.Tcid),
                UserType = e.UserType,
                TitleId = e.TitleId,
                Permissions = e.Permissions,
                PublicInstitutionName = string.IsNullOrEmpty(e.PublicInstitutionName)
                                            ? null
                                            : e.PublicInstitutionName
            });
        }

        public async Task ChangePasswordAsync(Guid userId, UpdateAdminPasswordDto dto)
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

        public async Task<AdminUserDto> UpdateAsync(Guid id, UpdateAdminDto dto)
        {
            // 1) Mevcut kaydı çek
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return null;

            // 2) Gelen verileri şifrele / encrypt et
            existing.Name = Crypto.Encrypt(dto.Name);
            existing.Surname = Crypto.Encrypt(dto.Surname);
            existing.Email = Crypto.Encrypt(dto.Email);
            existing.TitleId = dto.TitleId;

            // 3) Güncelle
            await _repo.UpdateAsync(id, existing);

            // 4) Dto’ya dönüştürüp dön
            return new AdminUserDto
            {
                Id = existing.Id,
                Name = Crypto.Decrypt(existing.Name),
                Surname = Crypto.Decrypt(existing.Surname),
                Email = Crypto.Decrypt(existing.Email),
                Tcid = Crypto.Decrypt(existing.Tcid),
                UserType = existing.UserType,
                TitleId = existing.TitleId,
                Permissions = existing.Permissions,
                PublicInstitutionName = string.IsNullOrEmpty(existing.PublicInstitutionName) ? null : Crypto.Decrypt(existing.PublicInstitutionName)
            };
        }
    }
}
