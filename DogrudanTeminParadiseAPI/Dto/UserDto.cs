namespace DogrudanTeminParadiseAPI.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Tcid { get; set; }
        public string UserType { get; set; }
        public List<string> Permissions { get; set; }
        public Guid? TitleId { get; set; }
    }
}
