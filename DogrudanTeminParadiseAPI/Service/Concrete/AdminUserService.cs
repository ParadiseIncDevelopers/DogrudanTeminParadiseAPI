using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
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
            // Hem Admin hem User’ı aynı koleksiyonda arıyoruz
            var all = await _repo.GetAllAsync();
            var user = all.FirstOrDefault(u =>
                u.Tcid == dto.Tcid &&
                u.Password == dto.Password);

            if (user == null)
                throw new UnauthorizedAccessException("Geçersiz TC veya parola");

            // Token üretimi
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Tcid),
                new Claim(ClaimTypes.Role, user.UserType)    // "Admin" veya "User"
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

            // 1. TC Kimlik No kontrolü
            bool tcidExists = allUsers.Any(u => u.Tcid == dto.Tcid);

            // 2. E-mail kontrolü
            bool emailExists = allUsers.Any(u => u.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase));

            if (tcidExists)
                throw new InvalidOperationException("Bu TC Kimlik Numarası zaten sistemde kayıtlı.");

            if (emailExists)
                throw new InvalidOperationException("Bu e-posta adresi zaten sistemde kayıtlı.");

            // Eğer sorun yoksa kullanıcıyı ekle
            var entity = _mapper.Map<AdminUser>(dto);
            entity.Id = Guid.NewGuid();

            await _repo.InsertAsync(entity);

            return _mapper.Map<AdminUserDto>(entity);
        }

        public async Task<AdminUserDto> GetByIdAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<AdminUserDto>(entity);
        }

        public async Task<IEnumerable<AdminUserDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(e => _mapper.Map<AdminUserDto>(e));
        }
    }
}
