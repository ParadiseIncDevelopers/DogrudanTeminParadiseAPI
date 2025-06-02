using DogrudanTeminParadiseAPI.Dto.Logger;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using DogrudanTeminParadiseAPI.Helpers.Options;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace DogrudanTeminParadiseAPI.Filter
{
    public class LogActionFilter : IAsyncActionFilter
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LoggerApiOptions _options;

        public LogActionFilter(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IOptions<LoggerApiOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _options = options.Value;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executedContext = await next();
            var httpContext = _httpContextAccessor.HttpContext;

            Guid? userGuid = null;
            string theToken = "";

            // 1) Eğer zaten authenticated ise claim’den al
            var idClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(idClaim, out var parsedId))
            {
                userGuid = parsedId;
            }
            else
            {
                // 2) Henüz authenticated değilse (login gibi), ObjectResult içinden token’ı al
                if (executedContext.Result is ObjectResult objectResult
                    && objectResult.Value is not null)
                {
                    theToken = JsonSerializer.Serialize(objectResult.Value,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                    try
                    {
                        using var doc = JsonDocument.Parse(theToken);
                        if (doc.RootElement.TryGetProperty("token", out var tokenProp))
                        {
                            var tokenString = tokenProp.GetString();
                            if (!string.IsNullOrEmpty(tokenString))
                            {
                                var handler = new JwtSecurityTokenHandler();
                                var jwt = handler.ReadJwtToken(tokenString);
                                var claimDict = jwt.Claims.ToDictionary(x => x.Type, y => y.Value);
                                var nameIdClaim = claimDict["nameid"];
                                if (Guid.TryParse(nameIdClaim, out var loginUserId))
                                {
                                    userGuid = loginUserId;
                                }
                            }
                        }
                    }
                    catch
                    {
                        // JSON parse veya token hatası olursa yoksay
                    }
                }
            }

            // 3) Log DTO oluştur
            var routeValues = context.ActionDescriptor.RouteValues;
            var controller = routeValues["controller"];
            var action = routeValues["action"];
            var method = httpContext.Request.Method;

            // Body içeriğini string olarak al
            string requestBody = GetRequestBody(httpContext);

            // Authorization header’dan alabiliyorsak, yoksa theToken kullan
            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();

            string? tokenToLog = "";
            try
            {
                tokenToLog = !string.IsNullOrEmpty(authHeader)
                ? authHeader.Replace("Bearer ", "")
                : JsonDocument.Parse(theToken).RootElement.TryGetProperty("token", out var t)
                    ? t.GetString()
                    : "";
            }
            catch (Exception) 
            {
                tokenToLog = "null";
                userGuid = Guid.Empty;
            }

            var logDto = new LogEntryDto
            {
                Id = Guid.NewGuid(),
                LogDateTime = DateTime.UtcNow,
                LogText = $"http {controller}/{action}",
                LogDescription = executedContext.Exception == null
                    ? "Success"
                    : executedContext.Exception.Message,
                LogObject = JsonSerializer.Serialize(new
                {
                    Controller = controller,
                    Action = action,
                    Query = httpContext.Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString()),
                    Body = requestBody
                }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                LogIP = "127.0.0.1",
                UserId = userGuid,
                Token = tokenToLog
            };

            // 4) CamelCase ile serileştirip Logger API'ye POST et
            var client = _httpClientFactory.CreateClient("LoggerApi");
            var camelOpts = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(logDto, camelOpts);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                await client.PostAsync("/api/Logs", content);
            }
            catch
            {
                // Log API hatası yoksay
            }
        }

        private string GetRequestBody(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();
                using var reader = new System.IO.StreamReader(
                    context.Request.Body,
                    encoding: Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true);
                var body = reader.ReadToEnd();
                context.Request.Body.Position = 0;
                return body;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
