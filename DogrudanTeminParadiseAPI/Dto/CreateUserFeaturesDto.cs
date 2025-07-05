using System.ComponentModel.DataAnnotations;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateUserFeaturesDto
    {
        public string ListName { get; set; }
        public List<FeatureDto> Features { get; set; } = new();
    }
}
