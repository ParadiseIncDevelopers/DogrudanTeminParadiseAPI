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
    public class SuperAdminService : ISuperAdminService
    {
        private readonly MongoDBRepository<SuperAdminUser> _repo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _cfg;

        public SuperAdminService(
            MongoDBRepository<SuperAdminUser> repo,
            IMapper mapper,
            IConfiguration cfg)
        {
            _repo = repo;
            _mapper = mapper;
            _cfg = cfg;
        }

        public async Task<string> AuthenticateAsync(LoginDto dto)
        {
            // 1) Tüm super-admin kullanıcıları çek
            var all = await _repo.GetAllAsync();
            var hashedInputPwd = Crypto.HashSha512(dto.Password);

            // 2) TC’yi AES decrypt edip, parola hash’i eşleştir
            var user = all.FirstOrDefault(u =>
                Crypto.Decrypt(u.Tcid) == dto.Tcid &&
                u.Password == hashedInputPwd);

            if (user == null)
                throw new UnauthorizedAccessException("Geçersiz TC veya parola.");

            // 3) JWT token oluştur
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, Crypto.Decrypt(user.Tcid)),
                new Claim(ClaimTypes.Role, user.UserType) // “SUPER_ADMIN”
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

        public async Task<SuperAdminDto> CreateAsync(CreateSuperAdminDto dto)
        {
            // 1) Aynı TC veya e-posta var mı kontrol et
            var allUsers = await _repo.GetAllAsync();
            var encTcid = Crypto.Encrypt(dto.Tcid);
            var encEmail = Crypto.Encrypt(dto.Email);

            if (allUsers.Any(u => u.Tcid == encTcid))
                throw new InvalidOperationException("Bu TC Kimlik Numarası zaten sistemde kayıtlı.");
            if (allUsers.Any(u => u.Email == encEmail))
                throw new InvalidOperationException("Bu e-posta adresi zaten sistemde kayıtlı.");

            // 2) DTO’dan entity’ye AutoMapper ile dönüştür (profilde encrypt, hash işlemleri tanımlı)
            var entity = _mapper.Map<SuperAdminUser>(dto);

            // 3) Kaydet
            await _repo.InsertAsync(entity);

            // 4) Kaydedilen entity’yi DTO’ya mapleyip döndür
            var result = _mapper.Map<SuperAdminDto>(entity);
            return result;
        }

        public async Task<SuperAdminDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;

            // Entity’den DTO’ya maple (profilde decrypt işlemi var)
            return _mapper.Map<SuperAdminDto>(e);
        }

        public async Task<IEnumerable<SuperAdminDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            // Her bir entity’yi DTO’ya mapleyip döndür
            return list.Select(e => _mapper.Map<SuperAdminDto>(e));
        }

        public async Task ChangePasswordAsync(Guid userId, UpdateAdminPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                throw new InvalidOperationException("Yeni parola ve onay eşleşmiyor.");

            var user = await _repo.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");

            // Mevcut parolayı doğrula
            var hashedCurrent = Crypto.HashSha512(dto.CurrentPassword);
            if (user.Password != hashedCurrent)
                throw new UnauthorizedAccessException("Mevcut parola yanlış.");

            // Yeni parolayı SHA512 ile hash’le ve güncelle
            user.Password = Crypto.HashSha512(dto.NewPassword);
            await _repo.UpdateAsync(userId, user);
        }

        public async Task UpdateAsync(Guid userId, UpdateSuperAdminDto dto)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");

            // Her alan güncellenmek istenirse null kontrolü yapıp AES Encrypt ile ata
            user.Name = Crypto.Encrypt(dto.Name);
            user.Surname = Crypto.Encrypt(dto.Surname);

            if (!string.IsNullOrEmpty(dto.Email))
                user.Email = Crypto.Encrypt(dto.Email);

            if (!string.IsNullOrEmpty(dto.Tcid))
                user.Tcid = Crypto.Encrypt(dto.Tcid);

            await _repo.UpdateAsync(userId, user);
        }
    }
}
