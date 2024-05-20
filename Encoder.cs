using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Security.Cryptography;

namespace Passwd
{
    internal static class Encoder
    {
        public static byte[] Encrypt(string originalText, byte[] key, byte[] iv)
        {
            byte[] encryptedText;

            using (Aes myAes = Aes.Create())
            {
                byte[] encrypted = EncryptStringToBytes_Aes(originalText, key, iv);

                encryptedText = encrypted;
            }

            return encryptedText;
        }

        public static string Decrypt(byte[] encryptedText, byte[] key, byte[] iv)
        {
            string originalText;

            using (Aes myAes = Aes.Create())
            {
                string roundtrip = DecryptStringFromBytes_Aes(encryptedText, key, iv);

                originalText = roundtrip.ToString();
            }
            originalText ??= "";

            return originalText;
        }

        public static byte[] GenerateKey()
        {
            Aes myAes = Aes.Create();
            myAes.GenerateKey();
            return myAes.Key;
        }

        private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0) throw new ArgumentNullException(nameof(plainText));
            if (Key == null || Key.Length <= 0) throw new ArgumentNullException(nameof(Key));
            if (IV == null || IV.Length <= 0) throw new ArgumentNullException(nameof(IV));
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }

        private static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0) throw new ArgumentNullException(nameof(cipherText));
            if (Key == null || Key.Length <= 0) throw new ArgumentNullException(nameof(Key));
            if (IV == null || IV.Length <= 0) throw new ArgumentNullException(nameof(IV));

            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
