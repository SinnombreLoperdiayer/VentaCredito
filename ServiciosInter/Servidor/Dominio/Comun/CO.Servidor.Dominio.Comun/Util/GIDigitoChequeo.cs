using System.Security.Cryptography;
using System.Text;

namespace CO.Servidor.Dominio.Comun.Util
{
  public class GIDigitoChequeo
  {
    private static readonly GIDigitoChequeo instancia = new GIDigitoChequeo();

    /// <summary>
    /// Retorna una instancia de la clase
    /// </summary>
    public static GIDigitoChequeo Instancia
    {
      get { return GIDigitoChequeo.instancia; }
    }

    /// <summary>
    /// Crea el digito de chequeo para un giro
    /// </summary>
    /// <param name="idGiro"></param>
    /// <returns></returns>
    public string CrearDigitoChequeo(long numeroGiro)
    {
      byte[] tmpSource;
      byte[] tmpHash;
      string semilla = "1a8t3fko9";
      //Crea una matriz de bytes de los datos de origen
      tmpSource = ASCIIEncoding.ASCII.GetBytes(numeroGiro.ToString() + semilla);

      //Calcula hash a partir de los datos de origen
      tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
      return ByteArrayToString(tmpHash);
    }

    private static string ByteArrayToString(byte[] arrInput)
    {
      int i;
      StringBuilder sOutput = new StringBuilder(arrInput.Length);
      for (i = 0; i < arrInput.Length - 1; i++)
      {
        sOutput.Append(arrInput[i].ToString("X2"));
      }
      return sOutput.ToString().Substring(0, 7);
    }

    /// <summary>
    /// Valida que el digito de chequeo corresponda al numero del giro
    /// </summary>
    /// <param name="numeroGiro"></param>
    /// <param name="digitoChequeo"></param>
    /// <returns></returns>
    public bool ValidarDigitoChequeo(long numeroGiro, string digitoChequeo)
    {
      string digito = CrearDigitoChequeo(numeroGiro);

      if (string.Compare(digito, digitoChequeo, true) == 0)
        return true;
      else
        return false;
    }
  }
}