using DogrudanTeminParadiseAPI.Factory.Abstract;
using DogrudanTeminParadiseAPI.Factory.Main;

namespace DogrudanTeminParadiseAPI.Factory.Concrete
{
    public class TeminApiExceptionFactory : ITeminApiExceptionFactory
    {
        public TeminApiException BadRequest(string message, string innerMessage = null)
            => new(400, message, innerMessage);

        public TeminApiException Unauthorized(string message, string innerMessage = null)
            => new(401, message, innerMessage);

        public TeminApiException Forbidden(string message, string innerMessage = null)
            => new(403, message, innerMessage);

        public TeminApiException NotFound(string message, string innerMessage = null)
            => new(404, message, innerMessage);

        public TeminApiException Conflict(string message, string innerMessage = null)
            => new(409, message, innerMessage);
    }
}
