namespace DogrudanTeminParadiseAPI.Dto
{
    public class UserAvatarDto
    {
        public Guid Id { get; set; }
        public Guid UserOrAdminId { get; set; }
        public int AvatarCode { get; set; }
    }
}
