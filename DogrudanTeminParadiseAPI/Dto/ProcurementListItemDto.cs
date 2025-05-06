namespace DogrudanTeminParadiseAPI.Dto
{
    public class ProcurementListItemDto
    {
        public Guid Id { get; set; }
        public Guid ProcurementEntryId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public Guid UnitId { get; set; }
    }
}
