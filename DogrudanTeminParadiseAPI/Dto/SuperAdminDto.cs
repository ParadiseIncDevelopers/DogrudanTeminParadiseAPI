namespace DogrudanTeminParadiseAPI.Dto
{
    public class SuperAdminDto
    {
        public string Username { get; set; }
        public string UserType { get; set; }
        public Dictionary<Guid, bool> ActivePassiveUsers { get; set; }
        public Dictionary<Guid, List<Guid>> AssignPermissionToAdmin { get; set; }
    }
}
