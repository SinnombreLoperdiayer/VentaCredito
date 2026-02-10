using CO.Servidor.Adminisiones.Mensajeria.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System.Data.SqlClient;

namespace CO.Servidor.Adminisiones.Mensajeria.Servicios
{
  /// <summary>
  /// Contiene la lógica de creación de una admisión rapienvio contra pago
  /// </summary>
  internal class ADAdmisionRapiEnvioContraPago : ADAdmisionServicio
  {
    #region Singleton

    public static readonly ADAdmisionRapiEnvioContraPago Instancia = new ADAdmisionRapiEnvioContraPago();

    #endregion Singleton

    #region Métodos

    /// <summary>
    /// Inserta admisión de un rapi envio contra pago
    /// </summary>
    /// <param name="idAdmisionMensajeria"></param>
    /// <param name="rapiEnvioContraPago"></param>
    public void AdicionarRapiEnvioContraPago(long idAdmisionMensajeria, ADRapiEnvioContraPagoDC rapiEnvioContraPago)
    {
      ADRepositorio.Instancia.AdicionarRapiEnvioContraPago(idAdmisionMensajeria, rapiEnvioContraPago, ControllerContext.Current.Usuario);
    }

    /// <summary>
    /// Inserta admisión de un rapi envio contra pago
    /// </summary>
    /// <param name="idAdmisionMensajeria"></param>
    /// <param name="rapiEnvioContraPago"></param>
    public void AdicionarRapiEnvioContraPago(long idAdmisionMensajeria, ADRapiEnvioContraPagoDC rapiEnvioContraPago,SqlConnection conexion, SqlTransaction transaccion)
    {
        ADRepositorio.Instancia.AdicionarRapiEnvioContraPago(idAdmisionMensajeria, rapiEnvioContraPago, ControllerContext.Current.Usuario,conexion,transaccion);
    }

    #endregion Métodos
  }
}