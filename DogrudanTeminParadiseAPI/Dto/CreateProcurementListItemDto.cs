namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateProcurementListItemDto
    {
        public Guid ProcurementEntryId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public Guid UnitId { get; set; }
    }
}
