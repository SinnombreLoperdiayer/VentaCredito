using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Framework.Servidor.Comun
{
  /// <summary>
  /// Clase para encripcion
  /// </summary>
  public class COEncripcion
  {
    /// <summary>
    /// Obtiene el valor hash desde una cadena de caracteres
    /// </summary>
    /// <param name="clave">Valor del cual se obtendra el hash</param>
    /// <returns>Clave hashed.</returns>
    public static string ObtieneHash(string clave)
    {
      if (!string.IsNullOrEmpty(clave))
      {
        byte[] tmpSource;
        byte[] tmpHash;
        tmpSource = ASCIIEncoding.ASCII.GetBytes(clave);
        tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);

        int i;
        StringBuilder sOutput = new StringBuilder(tmpHash.Length);
        for (i = 0; i < tmpHash.Length - 1; i++)
        {
          sOutput.Append(tmpHash[i].ToString("X2"));
        }
        return sOutput.ToString();
      }
      else
        throw new Exception("La clave no puede estar vacia");
    }
  }
}