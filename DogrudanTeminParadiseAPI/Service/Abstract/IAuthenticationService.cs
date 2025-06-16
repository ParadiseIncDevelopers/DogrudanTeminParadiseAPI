using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IAuthenticationService
    {
        Task<string> AuthenticateAsync(LoginDto dto);
        Task<int> GetTotalUserAndAdminCountAsync();
    }
}
