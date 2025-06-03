namespace DogrudanTeminParadiseAPI.Dto
{
    public class UpdateSuperAdminDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        /// <summary>
        /// Eğer e-posta güncellenecekse; düz metin gönderilecek, servis içinde AES Encrypt işlemi yapılacak
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Düz metin TC kimlik güncellemesi yapılmayacaksa null bırakılabilir. 
        /// Güncellenecekse düz metin verilerek AES Encrypt işlemi yapılacak.
        /// </summary>
        public string Tcid { get; set; }
    }
}
