namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateUserAvatarDto
    {
        public Guid UserOrAdminId { get; set; }
        public int AvatarCode { get; set; } = 10;
    }
}
