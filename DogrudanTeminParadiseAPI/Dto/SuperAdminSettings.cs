namespace DogrudanTeminParadiseAPI.Dto
{
    public class SuperAdminSettings
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string UserType { get; set; } = "SUPER_ADMIN";
    }
}
