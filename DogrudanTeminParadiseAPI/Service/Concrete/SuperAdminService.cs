using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Dto.Logger;
using DogrudanTeminParadiseAPI.Helpers;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class SuperAdminService : ISuperAdminService
    {
        private readonly MongoDBRepository<SuperAdminUser> _repo;
        private readonly MongoDBRepository<AdminUser> _adminRepo;  // to load all admins for status initialization
        private readonly MongoDBRepository<User> _userRepo;
        private readonly IMapper _mapper;
        private readonly SuperAdminSettings _settings;
        private readonly IConfiguration _cfg;

        public SuperAdminService(MongoDBRepository<SuperAdminUser> repo, MongoDBRepository<AdminUser> adminRepo, MongoDBRepository<User> userRepo, IMapper mapper, IOptions<SuperAdminSettings> opts, IConfiguration cfg)
        {
            _repo = repo;
            _adminRepo = adminRepo;
            _userRepo = userRepo;
            _mapper = mapper;
            _settings = opts.Value;
            _cfg = cfg;
        }

        public async Task<string> AuthenticateAsync(SuperAdminLoginDto dto)
        {
            var hashedInput = Crypto.HashSha512(dto.Password);
            if (_settings.Username != dto.Username || _settings.PasswordHash != hashedInput)
                throw new UnauthorizedAccessException("Invalid credentials.");

            SuperAdminUser? entity = new();
            try
            {
                entity = (await _repo.GetAllAsync()).FirstOrDefault()
                         ?? throw new InvalidOperationException("SuperAdmin metadata not found.");
            }
            catch 
            {
                SuperAdminUser superAdmin = new()
                {
                    Id = Guid.NewGuid(),
                    UserType = "SUPER_ADMIN",
                    ActivePassiveUsers = [],
                    AssignPermissionToAdmin = []
                };
                await _repo.InsertAsync(superAdmin);
                entity = (await _repo.GetAllAsync()).FirstOrDefault();
            }

            var handler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]);
            var token = handler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, entity.Id.ToString()),
                new Claim(ClaimTypes.Name, _settings.Username),
                new Claim(ClaimTypes.Role, entity.UserType)
            }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_cfg["Jwt:ExpiresInMinutes"])),
                Issuer = _cfg["Jwt:Issuer"],
                Audience = _cfg["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256)
            });
            return handler.WriteToken(token);
        }

        public async Task<SuperAdminDto> CreateAsync(CreateSuperAdminDto dto)
        {
            // Only one SuperAdminUser entity expected
            var entity = new SuperAdminUser
            {
                UserType = _settings.UserType
            };
            await _repo.InsertAsync(entity);
            return _mapper.Map<SuperAdminDto>(entity);
        }

        public async Task SetUserActiveStatusAsync(ChangeUserActiveStatusDto dto)
        {
            // Ensure system record exists
            var sys = (await _repo.GetAllAsync()).FirstOrDefault()
                      ?? throw new InvalidOperationException("System record not found.");

            // Populate default active entries for all admins
            var admins = await _adminRepo.GetAllAsync();
            foreach (var admin in admins)
            {
                var adminKey = admin.Id.ToString();
                if (!sys.ActivePassiveUsers.ContainsKey(adminKey))
                    sys.ActivePassiveUsers[adminKey] = true;
            }

            // Populate default active entries for all normal users
            var users = await _userRepo.GetAllAsync();
            foreach (var user in users)
            {
                var userKey = user.Id.ToString();
                if (!sys.ActivePassiveUsers.ContainsKey(userKey))
                    sys.ActivePassiveUsers[userKey] = true;
            }

            // Update specific target user status
            var targetKey = dto.TargetUserId.ToString();
            sys.ActivePassiveUsers[targetKey] = dto.IsActive;

            // Persist changes
            await _repo.UpdateAsync(sys.Id, sys);
        }

        public async Task AssignUsersToAdminAsync(AssignUsersToAdminDto dto)
        {
            var sys = (await _repo.GetAllAsync()).FirstOrDefault()
                      ?? throw new InvalidOperationException("System record not found.");

            var admins = await _adminRepo.GetAllAsync();
            foreach (var admin in admins)
            {
                if (!sys.AssignPermissionToAdmin.ContainsKey(admin.Id.ToString()))
                    sys.AssignPermissionToAdmin[admin.Id.ToString()] = [];
            }

            sys.AssignPermissionToAdmin[dto.AdminId.ToString()] = dto.PermittedUserIds;

            await _repo.UpdateAsync(sys.Id, sys);
        }

        public async Task<Dictionary<Guid, bool>> GetActivePassiveUsersAsync()
        {
            // Sistemdeki tek super-admin kaydını al
            var sys = (await _repo.GetAllAsync()).FirstOrDefault()
                      ?? throw new InvalidOperationException("System record not found.");

            // Entity’deki Dictionary<string,bool>’i Guid anahtarlı hale çevir
            var result = new Dictionary<Guid, bool>();
            foreach (var kv in sys.ActivePassiveUsers)
            {
                if (Guid.TryParse(kv.Key, out var guid))
                    result[guid] = kv.Value;
            }
            return result;
        }

        public async Task<Dictionary<Guid, List<Guid>>> GetAllAdminPermissionsAsync()
        {
            var sys = (await _repo.GetAllAsync()).FirstOrDefault()
                      ?? throw new InvalidOperationException("System record not found.");

            var dict = new Dictionary<Guid, List<Guid>>();
            foreach (var kv in sys.AssignPermissionToAdmin)
            {
                if (Guid.TryParse(kv.Key, out var adminId))
                {
                    var list = kv.Value
                        .Where(s => Guid.TryParse(s, out _))
                        .Select(s => Guid.Parse(s))
                        .ToList();
                    dict[adminId] = list;
                }
            }
            return dict;
        }

        public async Task<List<Guid>> GetAdminPermissionsAsync(Guid adminId)
        {
            var all = await GetAllAdminPermissionsAsync();
            if (!all.TryGetValue(adminId, out var list))
                throw new KeyNotFoundException("Admin ID not found in permissions.");
            return list;
        }

        public async Task ResetPasswordAsync(UpdateForgotPasswordDto dto)
        {
            // 1) AdminUser koleksiyonunda ara
            var allAdmins = await _adminRepo.GetAllAsync();
            var admin = allAdmins.FirstOrDefault(a => a.Id == dto.UserOrAdminId);
            if (admin != null)
            {
                admin.Password = Crypto.HashSha512(dto.NewPassword);
                await _adminRepo.UpdateAsync(admin.Id, admin);
                return;
            }

            // 2) Normal User koleksiyonunda ara
            var allUsers = await _userRepo.GetAllAsync();
            var user = allUsers.FirstOrDefault(u => u.Id == dto.UserOrAdminId);
            if (user != null)
            {
                user.Password = Crypto.HashSha512(dto.NewPassword);
                await _userRepo.UpdateAsync(user.Id, user);
                return;
            }

            // 3) Hiçbiri bulunamazsa
            throw new KeyNotFoundException("Kullanıcı veya admin bulunamadı.");
        }

        public async Task<IEnumerable<PageEntryDto>> GetPageActivitiesAsync(PageQueryParameters parameters)
        {
            // 1) BaseUrl’i al
            var baseUrl = _cfg["LoggerApi:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("LoggerApi BaseUrl ayarı eksik.");

            // 2) HttpClient oluştur ve sorgu string’ini hazırla
            using var client = new HttpClient { BaseAddress = new Uri(baseUrl) };

            var qs = new List<string>();
            if (parameters.From.HasValue) 
                qs.Add($"From={parameters.From.Value:o}");
            if (parameters.To.HasValue) 
                qs.Add($"To={parameters.To.Value:o}");
            if (parameters.UserId.HasValue) 
                qs.Add($"UserId={parameters.UserId.Value}");
            qs.Add($"Page={parameters.Page}");
            qs.Add($"PageSize={parameters.PageSize}");
            if (!string.IsNullOrEmpty(parameters.PageUrl))
                qs.Add($"PageUrl={Uri.EscapeDataString(parameters.PageUrl)}");

            var url = $"api/Page?{string.Join("&", qs)}";

            // 3) Çağrı yap
            var resp = await client.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"LoggerAPI çağrısı başarısız: {(int)resp.StatusCode}");

            // 4) Dönen JSON’u DTO listesine parse et
            var entries = await resp.Content.ReadFromJsonAsync<IEnumerable<PageEntryDto>>();
            return entries ?? [];
        }
    }
}