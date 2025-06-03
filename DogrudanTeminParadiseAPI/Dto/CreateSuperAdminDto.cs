namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateSuperAdminDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        /// <summary>
        /// Düz metin e-posta; servis içinde AES Encrypt metoduna verilecek
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Düz metin parola; servis içinde SHA512 ile hash’lenecek
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Düz metin TC kimlik numarası; servis içinde AES Encrypt metoduna verilecek
        /// </summary>
        public string Tcid { get; set; }
    }
}
