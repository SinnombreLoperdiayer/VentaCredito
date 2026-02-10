using CO.Servidor.Adminisiones.Mensajeria.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System.Data.SqlClient;

namespace CO.Servidor.Adminisiones.Mensajeria.Servicios
{
  /// <summary>
  /// Contiene la lógica de creación de una admisión de una notificación
  /// </summary>
  internal class ADAdmisionNotificacion : ADAdmisionServicio
  {
    #region Singleton

    public static readonly ADAdmisionNotificacion Instancia = new ADAdmisionNotificacion();

    #endregion Singleton

    #region Métodos

    /// <summary>
    /// Inserta la admisión de una notificación
    /// </summary>
    /// <param name="idAdmisionesMensajeria"></param>
    /// <param name="notificacion"></param>
    public void AdicionarNotificacion(long idAdmisionesMensajeria, ADNotificacion notificacion)
    {
      ADRepositorio.Instancia.AdicionarNotificacion(idAdmisionesMensajeria, notificacion, ControllerContext.Current.Usuario);
    }
    /// <summary>
    /// Inserta la admisión de una notificación
    /// </summary>
    /// <param name="idAdmisionesMensajeria"></param>
    /// <param name="notificacion"></param>
    public void AdicionarNotificacion(long idAdmisionesMensajeria, ADNotificacion notificacion,SqlConnection conexion, SqlTransaction transaccion)
    {
        ADRepositorio.Instancia.AdicionarNotificacion(idAdmisionesMensajeria, notificacion, ControllerContext.Current.Usuario,conexion, transaccion);
    }

    #endregion Métodos
  }
}