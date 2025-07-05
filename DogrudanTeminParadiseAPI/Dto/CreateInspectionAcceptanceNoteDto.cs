using System.ComponentModel.DataAnnotations;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateInspectionAcceptanceNoteDto
    {
        public Guid ProcurementEntryId { get; set; }
        public string Note { get; set; }
    }
}
