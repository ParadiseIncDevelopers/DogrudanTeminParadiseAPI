using System.ComponentModel.DataAnnotations;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class UpdateForgotPasswordDto
    {
        public Guid UserOrAdminId { get; set; }
        public string NewPassword { get; set; }
    }
}
