using System.Collections.Generic;
using CO.Servidor.Adminisiones.Mensajeria.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Adminisiones.Mensajeria.Servicios
{
  /// <summary>
  /// Contiene la lógica de creación de una admisión rapiradicado
  /// </summary>
  internal class ADAdmisionRapiRadicado : ADAdmisionServicio
  {
    #region Singleton

    public static readonly ADAdmisionRapiRadicado Instancia = new ADAdmisionRapiRadicado();

    #endregion Singleton

    #region Métodos

    /// <summary>
    /// Inserta admisión de un rapi radicado, puede tener mucha información de rapiradicado
    /// </summary>
    /// <param name="idAdmisionMensajeria"></param>
    /// <param name="rapiradicado"></param>
    public void AdicionarRapiRadicado(long idAdmisionMensajeria, ADRapiRadicado rapiradicado)
    {
      ADRepositorio.Instancia.AdicionarRapiRadicado(idAdmisionMensajeria, rapiradicado, ControllerContext.Current.Usuario);
    }

    #endregion Métodos
  }
}