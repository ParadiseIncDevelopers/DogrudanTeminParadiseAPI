using DogrudanTeminParadiseAPI.Filter;
using Microsoft.AspNetCore.Mvc;

namespace DogrudanTeminParadiseAPI.Helpers.Attributes
{
    public class CallLogsAttribute : TypeFilterAttribute
    {
        public CallLogsAttribute() : base(typeof(LogActionFilter)) { }
    }
}
