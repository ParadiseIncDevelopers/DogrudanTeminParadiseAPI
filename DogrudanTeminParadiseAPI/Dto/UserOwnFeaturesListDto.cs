namespace DogrudanTeminParadiseAPI.Dto
{
    public class UserOwnFeaturesListDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<UserFeaturesDto> FeaturesLists { get; set; }
    }
}
