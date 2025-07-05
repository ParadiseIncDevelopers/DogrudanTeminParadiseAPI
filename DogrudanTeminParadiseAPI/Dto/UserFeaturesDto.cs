namespace DogrudanTeminParadiseAPI.Dto
{
    public class UserFeaturesDto
    {
        public Guid Id { get; set; }
        public string ListName { get; set; }
        public List<FeatureDto> Features { get; set; }
    }
}
