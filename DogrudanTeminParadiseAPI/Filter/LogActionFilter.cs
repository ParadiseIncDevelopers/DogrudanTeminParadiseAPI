using DogrudanTeminParadiseAPI.Dto.Logger;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using DogrudanTeminParadiseAPI.Helpers.Options;

namespace DogrudanTeminParadiseAPI.Filter
{
    public class LogActionFilter : IAsyncActionFilter
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LoggerApiOptions _options;

        public LogActionFilter(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IOptions<LoggerApiOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _options = options.Value;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var executedContext = await next();

            var httpContext = _httpContextAccessor.HttpContext;
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userId, out var userGuid);

            var token = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var ip = httpContext.Connection.RemoteIpAddress?.ToString();

            var routeData = context.ActionDescriptor.RouteValues;
            var controller = routeData["controller"];
            var action = routeData["action"];
            var method = httpContext.Request.Method;
            var path = httpContext.Request.Path;

            var logDto = new LogEntryDto
            {
                Id = Guid.NewGuid(),
                LogDateTime = DateTime.UtcNow,
                LogText = $"{method} {controller}/{action}",
                LogDescription = executedContext.Exception == null
                    ? "Success"
                    : executedContext.Exception.Message,
                LogObject = path,
                LogIP = ip,
                UserId = userGuid == Guid.Empty ? null : userGuid,
                Token = token
            };

            var client = _httpClientFactory.CreateClient("LoggerApi");
            var json = JsonSerializer.Serialize(logDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                await client.PostAsync("/api/Logs", content);
            }
            catch
            {
                // Silently swallow any logging failures
            }
        }
    }
}
