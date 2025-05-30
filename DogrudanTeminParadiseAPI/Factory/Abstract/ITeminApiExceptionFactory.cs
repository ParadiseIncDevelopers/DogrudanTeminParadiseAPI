using DogrudanTeminParadiseAPI.Factory.Main;

namespace DogrudanTeminParadiseAPI.Factory.Abstract
{
    public interface ITeminApiExceptionFactory
    {
        TeminApiException BadRequest(string message, string innerMessage = null);
        TeminApiException Unauthorized(string message, string innerMessage = null);
        TeminApiException Forbidden(string message, string innerMessage = null);
        TeminApiException NotFound(string message, string innerMessage = null);
        TeminApiException Conflict(string message, string innerMessage = null);
    }
}
