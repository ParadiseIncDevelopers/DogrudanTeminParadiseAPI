using System.ComponentModel.DataAnnotations;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateUserOwnFeaturesListDto
    {
        public Guid UserId { get; set; }
        public List<CreateUserFeaturesDto> FeaturesLists { get; set; } = new();
    }
}
