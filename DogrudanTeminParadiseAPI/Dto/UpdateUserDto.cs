﻿
namespace DogrudanTeminParadiseAPI.Dto
{
    public class UpdateUserDto
    {
        public string Tcid { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public Guid? TitleId { get; set; }
        public string Password { get; set; }
        public List<string> Permissions { get; set; }
    }
}
