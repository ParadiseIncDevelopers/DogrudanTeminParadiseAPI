using System.ComponentModel.DataAnnotations;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class UpdateUserFeaturesDto
    {
        public Guid Id { get; set; }
        public string ListName { get; set; }
        public List<FeatureDto> Features { get; set; } = [];
    }
}
