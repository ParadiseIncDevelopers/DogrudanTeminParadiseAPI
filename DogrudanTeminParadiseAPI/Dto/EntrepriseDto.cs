namespace DogrudanTeminParadiseAPI.Dto
{
    public class EntrepriseDto
    {
        public Guid Id { get; set; }
        public string Unvan { get; set; }
        public string Vkn { get; set; }
        public string NaceKodu { get; set; }
        public string FirmaYetkilisi { get; set; }
        public int CalisanSayisi { get; set; }
        public string Address { get; set; }
        public string TaxOffice { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
