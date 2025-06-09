namespace DogrudanTeminParadiseAPI.Dto
{
    public class ChangeUserActiveStatusDto
    {
        public Guid TargetUserId { get; set; }
        public bool IsActive { get; set; }
    }
}
