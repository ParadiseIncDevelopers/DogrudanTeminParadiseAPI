namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateUserDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Tcid { get; set; }
        public string UserType { get; set; }
        public List<string> Permissions { get; set; }
        public Guid? TitleId { get; set; }
        public string PublicInstitutionName { get; set; }
    }
}
