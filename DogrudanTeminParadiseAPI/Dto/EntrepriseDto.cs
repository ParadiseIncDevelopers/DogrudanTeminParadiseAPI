namespace DogrudanTeminParadiseAPI.Dto
{
    public class EntrepriseDto
    {
        public Guid Id { get; set; }
        public string Unvan { get; set; }
        public string Vkn { get; set; }
        public string NaceKodu { get; set; }
        public string FirmaYetkilisi { get; set; }
        public List<Guid> VerilenMuayeneler { get; set; }
        public int CalisanSayisi { get; set; }
    }
}
