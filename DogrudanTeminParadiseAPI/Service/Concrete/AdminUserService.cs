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
        private readonly MongoDBRepository<SuperAdminUser> _sysRepo;
        private readonly MongoDBRepository<Title> _titleRepo;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _cfg;

        public AdminUserService(
            MongoDBRepository<AdminUser> repo,
            MongoDBRepository<SuperAdminUser> sysRepo,
            MongoDBRepository<Title> titleRepo,
            IUserService userService,
            IMapper mapper,
            IConfiguration cfg)
        {
            _repo = repo;
            _sysRepo = sysRepo;
            _titleRepo = titleRepo;
            _userService = userService;
            _mapper = mapper;
            _cfg = cfg;
        }

        public async Task<string> AuthenticateAsync(LoginDto dto)
        {
            // 1) Check admin DB
            var allAdmins = await _repo.GetAllAsync();
            var hashedPwd = Crypto.HashSha512(dto.Password);
            var admin = allAdmins.FirstOrDefault(u => Crypto.Decrypt(u.Tcid) == dto.Tcid);

            if (admin != null)
            {
                // 1a) Permission check via system ActivePassiveUsers
                var sys = (await _sysRepo.GetAllAsync()).FirstOrDefault()
                          ?? throw new InvalidOperationException("System record not found.");
                var key = admin.Id.ToString();
                if (sys.ActivePassiveUsers.TryGetValue(key, out var isActive) && !isActive)
                    throw new UnauthorizedAccessException("Admin kullanıcı pasif; erişim yok.");

                // 1b) Password check
                if (admin.Password != hashedPwd)
                    throw new UnauthorizedAccessException("Admin parola hatalı.");

                // 1c) Generate JWT for admin
                var tokenHandler = new JwtSecurityTokenHandler();
                var keyBytes = Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]);
                var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                    new Claim(ClaimTypes.Name,         Crypto.Decrypt(admin.Tcid)),
                    new Claim(ClaimTypes.Role,         admin.UserType)
                }),
                    Expires = DateTime.UtcNow.AddMinutes(int.Parse(_cfg["Jwt:ExpiresInMinutes"])),
                    Issuer = _cfg["Jwt:Issuer"],
                    Audience = _cfg["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256)
                });
                return tokenHandler.WriteToken(token);
            }

            // 2) Not an admin: try userService for normal user
            try
            {
                return await _userService.AuthenticateAsync(dto);
            }
            catch (KeyNotFoundException)
            {
                // user not found in either DB
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");
            }
        }

        public async Task<AdminUserDto> CreateAsync(CreateUserDto dto)
        {
            var allUsers = await _repo.GetAllAsync();

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

            if (allUsers.Any(u => u.Tcid == encTcid))
                throw new InvalidOperationException("Bu TC Kimlik Numarası zaten sistemde kayıtlı.");
            if (allUsers.Any(u => u.Email == encEmail))
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
    }
}
