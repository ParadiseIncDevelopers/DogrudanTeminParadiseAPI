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
        private readonly IMapper _mapper;
        private readonly IConfiguration _cfg;

        public AdminUserService(
            MongoDBRepository<AdminUser> repo,
            IMapper mapper,
            IConfiguration cfg)
        {
            _repo = repo;
            _mapper = mapper;
            _cfg = cfg;
        }

        public async Task<string> AuthenticateAsync(LoginDto dto)
        {
            var all = await _repo.GetAllAsync();
            var hashedInputPwd = Crypto.HashSha512(dto.Password);

            var user = all.FirstOrDefault(u =>
                Crypto.Decrypt(u.Tcid) == dto.Tcid &&
                u.Password == hashedInputPwd);

            if (user == null)
                throw new UnauthorizedAccessException("Geçersiz TC veya parola");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, Crypto.Decrypt(user.Tcid)),
                    new Claim(ClaimTypes.Role, user.UserType)
                }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_cfg["Jwt:ExpiresInMinutes"])),
                Issuer = _cfg["Jwt:Issuer"],
                Audience = _cfg["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            });

            return tokenHandler.WriteToken(token);
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
                                            : Crypto.Decrypt(e.PublicInstitutionName)
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
    }
}
