namespace DogrudanTeminParadiseAPI.Dto
{
    public class SuperAdminDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }

        /// <summary>
        /// AES ile şifrelenmiş olarak saklanan e-posta, dışarı dökmeden önce deşifre edilecek
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// AES ile şifrelenmiş olarak saklanan TC kimlik numarası, dışarı dökmeden önce deşifre edilecek
        /// </summary>
        public string Tcid { get; set; }

        /// <summary>
        /// Bu her zaman "SUPER_ADMIN" olacak
        /// </summary>
        public string UserType { get; set; }
    }
}
