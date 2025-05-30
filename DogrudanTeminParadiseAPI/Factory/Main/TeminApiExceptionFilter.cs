using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DogrudanTeminParadiseAPI.Factory.Main
{
    public class TeminApiExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is TeminApiException tex)
            {
                context.Result = new ObjectResult(tex.Payload)
                {
                    StatusCode = tex.StatusCode
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
