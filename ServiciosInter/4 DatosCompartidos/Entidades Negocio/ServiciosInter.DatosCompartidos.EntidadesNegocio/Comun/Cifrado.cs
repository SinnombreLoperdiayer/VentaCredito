using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Crypto.AES;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Comun
{
    public static class Cifrado
    {
        private static readonly int iterations = 1000;
        /// <summary>
        /// Cifra de Aes 256 a texto
        /// </summary>
        /// <param name="texto"></param>
        /// <returns></returns>
        public static string EncriptarTexto(string texto)
        {
            byte[] encrypted;
            byte[] IV;
            byte[] Salt = ObtenerSalto();
            byte[] Key = CrearLlave(RecursosMensajeria.LlaveAES256, Salt);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Mode = CipherMode.CBC;

                aesAlg.GenerateIV();
                IV = aesAlg.IV;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(texto);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            byte[] combinedIvSaltCt = new byte[Salt.Length + IV.Length + encrypted.Length];
            Array.Copy(Salt, 0, combinedIvSaltCt, 0, Salt.Length);
            Array.Copy(IV, 0, combinedIvSaltCt, Salt.Length, IV.Length);
            Array.Copy(encrypted, 0, combinedIvSaltCt, Salt.Length + IV.Length, encrypted.Length);

            return Convert.ToBase64String(combinedIvSaltCt.ToArray());
        }

        public static byte[] CrearLlave(string llave, byte[] salto)
        {
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(llave, salto, iterations))
                return rfc2898DeriveBytes.GetBytes(32);
        }

        private static byte[] ObtenerSalto()
        {
            var salt = new byte[32];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return salt;
        }
        /// <summary>
        /// Decifra de Aes 256 a texto
        /// </summary>
        /// <param name="texto"></param>
        /// <returns></returns>
        public static string DesencriptarTexto(string texto)
        {
          byte[] inputAsByteArray;
            string plaintext = null;
            try
            {
                inputAsByteArray = Convert.FromBase64String(texto);

                byte[] Salt = new byte[32];
                byte[] IV = new byte[16];
                byte[] Encoded = new byte[inputAsByteArray.Length - Salt.Length - IV.Length];

                Array.Copy(inputAsByteArray, 0, Salt, 0, Salt.Length);
                Array.Copy(inputAsByteArray, Salt.Length, IV, 0, IV.Length);
                Array.Copy(inputAsByteArray, Salt.Length + IV.Length, Encoded, 0, Encoded.Length);

                byte[] Key = CrearLlave(RecursosMensajeria.LlaveAES256, Salt);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(Encoded))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }

                return plaintext;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        /// <summary>
        /// Metodo Para Cifrar Cadenas con AES256
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string Encrypt(string encryptString)
        {
            encryptString = Regex.Replace(encryptString, @"\s", "");
            if (string.IsNullOrEmpty(encryptString))
            {
                return null;
            }
            else
            {
                string encript = "";
                string valorNormalizado = Regex.Replace(encryptString.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 *#@-]+", "");
                try
                {
                    encript = AES.EncryptString(RecursosMensajeria.LlaveAES256, valorNormalizado);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return encript;
            }
        }

        /// <summary>
        /// Metodo Para Descifrar Cadenas con AES256
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return null;
            }
            else
            {
                string decript = "";
                try
                {
                    decript = AES.DecryptString(RecursosMensajeria.LlaveAES256, cipherText);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return decript;
            }
        }
    }
}
