using System.Security.Cryptography;
using System.Text;

namespace CO.Servidor.Admisiones.Giros.Comun.Utilidades
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
    public string CrearDigitoChequeo(long numGiro)
    {
      byte[] tmpSource;
      byte[] tmpHash;
      string semilla = "123456879";
      //Crea una matriz de bytes de los datos de origen
      tmpSource = ASCIIEncoding.ASCII.GetBytes(numGiro.ToString() + semilla);

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
      return sOutput.ToString();
    }
  }
}