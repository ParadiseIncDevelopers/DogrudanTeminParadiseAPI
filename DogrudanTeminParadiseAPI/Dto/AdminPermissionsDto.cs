namespace DogrudanTeminParadiseAPI.Dto
{
    public class AdminPermissionsDto
    {
        public Guid AdminId { get; set; }
        public List<Guid> PermittedUserIds { get; set; }
    }
}
