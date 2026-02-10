using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Framework.Servidor.Seguridad
{
  /// <summary>
  /// Diccionario autorrenovable
  /// </summary>
  internal class DiccionarioCredenciales : Dictionary<string, DateTime>
  {
    public static readonly DiccionarioCredenciales instancia = new DiccionarioCredenciales();

    #region Atributos

    /// <summary>
    /// Tiempo de expiración de la credencial dentro del diccionario en segundos
    /// </summary>
    private const double tiempoExpiracion = 3600;

    /// <summary>
    /// Lista de las llaves del diccionario a borrar porque su tiempo de vida ha caducado
    /// </summary>
    private List<string> llavesABorrar;

    #endregion Atributos

    #region Constructor

    /// <summary>
    /// Constructor del diccionario
    /// </summary>
    private DiccionarioCredenciales()
    {
      llavesABorrar = new List<string>();

      this.Start();
    }

    #endregion Constructor

    #region Métodos privados

    /// <summary>
    /// Método que inicia
    /// </summary>
    private void Start()
    {
      //Lanzar hilo del metodo ValidacionExpiracionCredenciales
      Thread supervisor = new Thread(new ParameterizedThreadStart(delegate { this.ValidacionExpiracionCredenciales(); }));
      supervisor.Start();
    }

    /// <summary>
    /// Hace la validación de la vida de las credenciales, remueve las credenciales cuyo tiempo de vida ha caducado
    /// </summary>
    private void ValidacionExpiracionCredenciales()
    {
      while (true)
      {
        foreach (KeyValuePair<string, DateTime> credencial in this)
        {
          if (credencial.Value.AddSeconds(tiempoExpiracion) < DateTime.Now)
          {
            this.llavesABorrar.Add(credencial.Key);
          }
        }

        //this.llavesABorrar.ForEach(key => this.Remove(key));
        foreach (string key in this.llavesABorrar)
        {
          if (key != null && this.ContainsKey(key))
            this.Remove(key);
        }
        this.llavesABorrar = new List<string>();
        System.Threading.Thread.Sleep(5000);
      }
    }

    #endregion Métodos privados
  }
}