namespace DogrudanTeminParadiseAPI.Factory.Main
{
    public class TeminApiException : Exception
    {
        public int StatusCode { get; }
        public object Payload { get; }

        public TeminApiException(int statusCode, string message, string innerMessage = null)
            : base(message, innerMessage != null ? new Exception(innerMessage) : null)
        {
            StatusCode = statusCode;
            Payload = new
            {
                error = message,
                details = innerMessage
            };
        }
    }
}
