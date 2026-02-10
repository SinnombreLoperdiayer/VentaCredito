using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Framework.Servidor.Seguridad
{
     /// <summary>
    /// clase para encriptar y desencriptar textos utilizando un algoritmo AES
    /// </summary>
    public sealed class AlgoritmoEncripcion
    {
        public static readonly AlgoritmoEncripcion Instance = new AlgoritmoEncripcion();

        private RijndaelManaged algoritEncripcion;

        private AlgoritmoEncripcion()
        {
            algoritEncripcion = new RijndaelManaged();           
        }
        /// <summary>
        /// Encriptar un string utilizando un algoritmo AES
        /// </summary>
        /// <param name="Texto"></param>
        /// <returns>String para ser encriptado</returns>
        public string EncriptarCadena(string cadenaPlana)
        {
            byte[] textoBytes = Encoding.Default.GetBytes(cadenaPlana);
            string mensajeEncriptado = Encriptar(textoBytes, algoritEncripcion);
            return mensajeEncriptado;
        }


        /// <summary>
        /// Desencriptar un string utilizando un algoritmo AES
        /// </summary>
        /// <param name="Texto"></param>
        /// <returns>String desencriptado</returns>
        public string DesencriptarCadena(string cadenaEncriptada)
        {
            byte[] textoBytes = Convert.FromBase64String(cadenaEncriptada);
            string mensajeDesencriptado = Desencriptar(textoBytes, algoritEncripcion);
            return mensajeDesencriptado;
        }

        /// <summary>
        /// Configura el algoritmo utilizado para la encripcion y desencripcion
        /// </summary>
        /// <param name="comportamientoAlgoritmo">Indica el comportamiento del algoritmo (Encriptar y Desencriptar)</param>
        /// <returns>Algoritmo Configurado</returns>
        private ICryptoTransform ConfigAlgoritmo(ComportamientoAlgoritmo comportamientoAlgoritmo)
        {
            //Debe ser una cadena de 16 bytes, es decir, 16 caracteres
            string InitialVector = "ldoe9487fnvhydqs";
            //Puede ser cualquier cadena
            string SaltValue = "kjwkejekjkwkrwekrwkñlñlksdf9ok7s67554";
            string Password = "s989sd90fioiosdf8998sdi9898ñ23lkñ2k34ñ";
            int KeySize = 256;
            string hashAlgorithm = "MD5";
            int PasswordIterations = 2;

            byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(SaltValue);

            PasswordDeriveBytes password = new PasswordDeriveBytes(Password, saltValueBytes, hashAlgorithm, PasswordIterations);
            byte[] keyBytes = password.GetBytes(KeySize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;

            ICryptoTransform cryptor;

            if (comportamientoAlgoritmo == ComportamientoAlgoritmo.Encriptar)
                cryptor = symmetricKey.CreateEncryptor(keyBytes, InitialVectorBytes);
            else
                cryptor = symmetricKey.CreateDecryptor(keyBytes, InitialVectorBytes);

            return cryptor;
        }

      /// <summary>
      /// Encripta un mensaje plano
      /// </summary>
      /// <param name="mensajeSinEncriptar">array de bytes para encriptar</param>
      /// <param name="algoritmo">Algoritmo utilizado para la encripcion</param>
      /// <returns>Mensaje encriptado</returns>
        private string Encriptar(byte[] mensajeSinEncriptar, RijndaelManaged algoritmo)
        {
            ICryptoTransform encriptador = ConfigAlgoritmo(ComportamientoAlgoritmo.Encriptar);
            // Creamos un MemoryStream  
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encriptador, CryptoStreamMode.Write);
            // Escribimos el textoPlano hacia el CryptoStream  
            cryptoStream.Write(mensajeSinEncriptar, 0, mensajeSinEncriptar.Length);
            // Terminamos la operación de encriptación.  
            cryptoStream.FlushFinalBlock();
            byte[] TextoBytes = memoryStream.ToArray();
            // Liberamos.  
            memoryStream.Close();
            cryptoStream.Close();

            return Convert.ToBase64String(TextoBytes);
        }

        /// <summary>
        /// Desencripta un mensaje 
        /// </summary>
        /// <param name="mensajeEncriptado">array de bytes para desencriptar</param>
        /// <param name="algoritmo">Algoritmo utilizado para la desencripcion</param>
        /// <returns>Mensaje desencriptado</returns>
        private string Desencriptar(byte[] mensajeEncriptado, RijndaelManaged algoritmo)
        {

            int numeroBytesDesencriptados = 0;
            byte[] mensajeDesencriptado = new byte[mensajeEncriptado.Length];

            ICryptoTransform desencriptador = ConfigAlgoritmo(ComportamientoAlgoritmo.Desencriptar);
            // Procedemos a descifrar el mensaje  
            MemoryStream memoryStream = new MemoryStream(mensajeEncriptado);
            // Creamos el CryptoStream  
            CryptoStream cryptoStream = new CryptoStream(memoryStream, desencriptador, CryptoStreamMode.Read);
            // Decrypting data and get the count of plain text bytes.  
            numeroBytesDesencriptados = cryptoStream.Read(mensajeDesencriptado, 0, mensajeDesencriptado.Length);
            // Liberamos recursos.  
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(mensajeDesencriptado, 0, numeroBytesDesencriptados);
        }
    }
    public enum ComportamientoAlgoritmo
    {
        Encriptar,
        Desencriptar
    }

}
