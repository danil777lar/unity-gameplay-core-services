using System;
using System.IO;
using System.Security.Cryptography;

namespace Larje.Core.Saves
{
    public static class AesUtility
    {
        public static string GenerateKey()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                return Convert.ToBase64String(aes.Key);
            }
        }

        public static string GenerateIv()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateIV();
                return Convert.ToBase64String(aes.IV);
            }
        }

        public static string Encrypt(string text, string key, string iv)
        {
            using (Aes aes = Aes.Create())
            using (ICryptoTransform encryptor = aes.CreateEncryptor(Convert.FromBase64String(key), Convert.FromBase64String(iv)))
            using (MemoryStream memory = new MemoryStream())
            {
                using (CryptoStream crypto = new CryptoStream(memory, encryptor, CryptoStreamMode.Write))
                using (StreamWriter writer = new StreamWriter(crypto))
                {
                    writer.Write(text);
                }

                return Convert.ToBase64String(memory.ToArray());
            }
        }

        public static string Decrypt(string text, string key, string iv)
        {
            using (Aes aes = Aes.Create())
            using (ICryptoTransform decryptor = aes.CreateDecryptor(Convert.FromBase64String(key), Convert.FromBase64String(iv)))
            using (MemoryStream memory = new MemoryStream(Convert.FromBase64String(text)))
            using (CryptoStream crypto = new CryptoStream(memory, decryptor, CryptoStreamMode.Read))
            using (StreamReader reader = new StreamReader(crypto))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
