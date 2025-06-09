using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
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

            var entity = (await _repo.GetAllAsync()).FirstOrDefault()
                         ?? throw new InvalidOperationException("SuperAdmin metadata not found.");

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

        //BUNU ŞİMDİLİK KULLANMAYACAĞIZ : yeni versiyonda değişik bir şeyler düşüneceğim.
        public async Task<SystemActivityDto> GetSystemActivityAsync()
        {
            var sys = (await _repo.GetAllAsync()).FirstOrDefault();
            if (sys == null)
                return new SystemActivityDto
                {
                    UserStatuses = new List<UserStatusDto>(),
                    AdminPermissions = new List<AdminPermissionsDto>()
                };

            var statuses = sys.ActivePassiveUsers
                .Select(kv => new UserStatusDto { UserId = Guid.Parse(kv.Key), IsActive = kv.Value })
                .ToList();
            var perms = sys.AssignPermissionToAdmin
                .Select(kv => new AdminPermissionsDto { AdminId = Guid.Parse(kv.Key), PermittedUserIds = kv.Value })
                .ToList();
            return new SystemActivityDto { UserStatuses = statuses, AdminPermissions = perms };
        }
    }

    
}