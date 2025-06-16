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
    public class AuthenticationService : IAuthenticationService
    {
        private readonly MongoDBRepository<AdminUser> _adminRepo;
        private readonly MongoDBRepository<User> _userRepo;
        private readonly MongoDBRepository<SuperAdminUser> _sysRepo;
        private readonly IConfiguration _cfg;

        public AuthenticationService(
            MongoDBRepository<AdminUser> adminRepo,
            MongoDBRepository<User> userRepo,
            MongoDBRepository<SuperAdminUser> sysRepo,
            IConfiguration cfg)
        {
            _adminRepo = adminRepo;
            _userRepo = userRepo;
            _sysRepo = sysRepo;
            _cfg = cfg;
        }

        public async Task<string> AuthenticateAsync(LoginDto dto)
        {
            var hashed = Crypto.HashSha512(dto.Password);

            // 1) Önce AdminUser DB’de dene
            var adminList = await _adminRepo.GetAllAsync();
            var admin = adminList.FirstOrDefault(u => Crypto.Decrypt(u.Tcid) == dto.Tcid);
            if (admin != null)
            {
                // Pasiflik kontrolü
                var sys = (await _sysRepo.GetAllAsync()).FirstOrDefault()
                          ?? throw new InvalidOperationException("System record not found.");
                var key = admin.Id.ToString();
                if (sys.ActivePassiveUsers.TryGetValue(key, out var isActive) && !isActive)
                    throw new UnauthorizedAccessException("Admin kullanıcı pasif; erişim yok.");

                // Parola kontrolü
                if (admin.Password != hashed)
                    throw new UnauthorizedAccessException("Admin parola hatalı.");

                // Token üret
                return CreateJwtToken(admin.Id, Crypto.Decrypt(admin.Tcid), admin.UserType);
            }

            // 2) Admin değilse normal User DB’de dene
            var userList = await _userRepo.GetAllAsync();
            var user = userList.FirstOrDefault(u => Crypto.Decrypt(u.Tcid) == dto.Tcid);
            if (user != null)
            {
                // Pasiflik kontrolü
                var sys = (await _sysRepo.GetAllAsync()).FirstOrDefault()
                          ?? throw new InvalidOperationException("System record not found.");
                var ukey = user.Id.ToString();
                if (sys.ActivePassiveUsers.TryGetValue(ukey, out var isActiveU) && !isActiveU)
                    throw new UnauthorizedAccessException("Kullanıcı pasif; erişim yok.");

                // Parola kontrolü
                if (user.Password != hashed)
                    throw new UnauthorizedAccessException("Kullanıcı parola hatalı.");

                return CreateJwtToken(user.Id, Crypto.Decrypt(user.Tcid), user.UserType);
            }

            // Ne admin ne user
            throw new KeyNotFoundException("Kullanıcı bulunamadı.");
        }

        public async Task<int> GetTotalUserAndAdminCountAsync()
        {
            // SuperAdmin haricinde:
            var adminCount = (await _adminRepo.GetAllAsync()).Count();
            var userCount = (await _userRepo.GetAllAsync()).Count();
            return adminCount + userCount;
        }

        private string CreateJwtToken(Guid id, string nameClaim, string role)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]);
            var token = handler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name,           nameClaim),
                new Claim(ClaimTypes.Role,           role)
            }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_cfg["Jwt:ExpiresInMinutes"])),
                Issuer = _cfg["Jwt:Issuer"],
                Audience = _cfg["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            });
            return handler.WriteToken(token);
        }
    }
}
