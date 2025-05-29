using System.Security.Cryptography;
using System.Text;

namespace DogrudanTeminParadiseAPI.Helpers
{
    public static class Crypto
    {
        // AES için sabit IV (16 byte)
        private static readonly byte[] _aesIv = new byte[16];

        // Appsettings'den gelen 256-bit anahtar
        private static readonly byte[] _aesKey;

        // Static constructor: appsettings.json'dan anahtarı okur
        static Crypto()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            var keyString = config["EncryptionKey"];
            if (string.IsNullOrEmpty(keyString) || keyString.Length != 32)
                throw new InvalidOperationException("Geçersiz veya eksik EncryptionKey. 32 karakter uzunluğunda olmalı.");

            _aesKey = Encoding.UTF8.GetBytes(keyString);
        }

        /// <summary>
        /// SHA-512 ile hash hesaplar ve hex string olarak döner.
        /// </summary>
        public static string HashSha512(string input)
        {
            var hash = SHA512.HashData(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// AES-256 CBC ile metni şifreler ve Base64 string olarak döner.
        /// </summary>
        public static string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _aesKey;
            aes.IV = _aesIv;
            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs, Encoding.UTF8))
            {
                sw.Write(plainText);
            }
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// AES-256 CBC ile şifrelenmiş Base64 metni çözer.
        /// </summary>
        public static string Decrypt(string cipherText)
        {
            var buffer = Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();
            aes.Key = _aesKey;
            aes.IV = _aesIv;
            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(buffer);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs, Encoding.UTF8);
            return sr.ReadToEnd();
        }
    }
}
