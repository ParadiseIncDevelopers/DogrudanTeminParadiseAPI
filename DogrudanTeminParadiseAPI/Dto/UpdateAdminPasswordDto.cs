namespace DogrudanTeminParadiseAPI.Dto
{
    public class UpdateAdminPasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
