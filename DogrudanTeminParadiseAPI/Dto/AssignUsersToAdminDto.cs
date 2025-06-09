namespace DogrudanTeminParadiseAPI.Dto
{
    public class AssignUsersToAdminDto
    {
        public Guid AdminId { get; set; }
        public List<string> PermittedUserIds { get; set; }
    }
}
