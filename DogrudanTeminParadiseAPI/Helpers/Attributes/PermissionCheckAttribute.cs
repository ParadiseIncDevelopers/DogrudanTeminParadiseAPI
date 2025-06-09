using DogrudanTeminParadiseAPI.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;
using DogrudanTeminParadiseAPI.Filter;

namespace DogrudanTeminParadiseAPI.Helpers.Attributes
{
    public class PermissionCheckAttribute : TypeFilterAttribute
    {
        public PermissionCheckAttribute() : base(typeof(PermissionCheckFilter)) { }
    }
}
