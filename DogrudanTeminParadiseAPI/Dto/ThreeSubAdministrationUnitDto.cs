
namespace DogrudanTeminParadiseAPI.Dto
{
    public class ThreeSubAdministrationUnitDto
    {
        public Guid Id { get; set; }
        public Guid AdministrationUnitId { get; set; }
        public Guid SubAdministrationUnitId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public Guid CreatedByAdminId { get; set; }
    }
}
