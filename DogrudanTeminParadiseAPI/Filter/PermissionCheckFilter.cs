using DogrudanTeminParadiseAPI.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;

namespace DogrudanTeminParadiseAPI.Filter
{
    public class PermissionCheckFilter : IAsyncAuthorizationFilter
    {
        // Bu isimleri DI ile resolve edeceğiz
        private readonly IMongoCollection<SuperAdminUser> _sysCollection;
        private readonly IMongoCollection<ProcurementEntry> _entryCollection;

        public PermissionCheckFilter(IMongoDatabase db)
        {
            // DI container’ınızda IMongoDatabase kayıtlı olmalı
            _sysCollection = db.GetCollection<SuperAdminUser>("SuperAdmin");
            _entryCollection = db.GetCollection<ProcurementEntry>("ProcurementEntries");
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // 1) Kimlik bilgilerinden userId & role al
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var role = user.FindFirstValue(ClaimTypes.Role);
            var id = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            // 2) Role’a göre iki farklı akış
            if (string.Equals(role, "ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                // 2a) SuperAdminUser ayarlarını çek
                var sys = await _sysCollection
                    .Find(FilterDefinition<SuperAdminUser>.Empty)
                    .FirstOrDefaultAsync();
                if (sys == null)
                {
                    context.Result = new ForbidResult();
                    return;
                }

                // 2b) Eğer bu AdminUser için hiç key yoksa => erişim yok
                if (!sys.AssignPermissionToAdmin.TryGetValue(id.ToString(), out var permittedList))
                {
                    context.Result = new ForbidResult();
                    return;
                }

                // 2c) GET ALL isAllowed? Biz sadece kendi izin verdiği entryId’leri görecek
                var routeId = GetRouteGuid(context, "id");
                if (routeId == null)
                {
                    // Örneğin GetAll metodu: sorgu parametresi olmadığı için
                    // buraya girmez, Controller-level kodunuz query sonucu
                    // doğrudan service.GetAllAsync(permittedList) yapmalı.
                    return;
                }

                // 2d) GET BY ID / UPDATE / DELETE
                if (!permittedList.Contains(routeId.Value.ToString()))
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
            else if (string.Equals(role, "USER", StringComparison.OrdinalIgnoreCase))
            {
                // 3) Normal kullanıcı sadece kendi kayıtlarını görebilir
                var routeId = GetRouteGuid(context, "requesterId")   // by-requester gibi
                            ?? GetRouteGuid(context, "userId")       // generic param
                            ?? GetRouteGuid(context, "id");          // varsa
                if (routeId.HasValue && routeId.Value != id)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
            // SüperAdmin vs “SUPER_ADMIN” rolü burada kontrol edilmiyor:
            // ona tüm izinler zaten daha yüksek seviyede verildi.
        }

        private Guid? GetRouteGuid(AuthorizationFilterContext ctx, string key)
        {
            if (ctx.RouteData.Values.TryGetValue(key, out var obj)
             && Guid.TryParse(obj?.ToString(), out var gid))
                return gid;
            // Eğer model binding/body üzerinden alacaksanız, burada
            // ctx.ActionArguments’den de çekebilirsiniz.
            return null;
        }
    }
}
