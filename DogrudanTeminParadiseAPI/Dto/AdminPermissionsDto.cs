namespace DogrudanTeminParadiseAPI.Dto
{
    public class AdminPermissionsDto
    {
        public Guid AdminId { get; set; }
        public List<string> PermittedUserIds { get; set; }
    }
}
