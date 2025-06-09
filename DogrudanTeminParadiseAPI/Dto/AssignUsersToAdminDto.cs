namespace DogrudanTeminParadiseAPI.Dto
{
    public class AssignUsersToAdminDto
    {
        public Guid AdminId { get; set; }
        public List<Guid> PermittedUserIds { get; set; }
    }
}
