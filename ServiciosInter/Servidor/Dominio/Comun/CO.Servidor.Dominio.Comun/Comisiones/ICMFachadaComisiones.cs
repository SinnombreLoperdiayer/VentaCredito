using System;
using System.Collections.Generic;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using System.Data.SqlClient;

namespace CO.Servidor.Dominio.Comun.Comisiones
{
  /// <summary>
  ///  Interfaz de la fachada de comisiones
  /// </summary>
  public interface ICMFachadaComisiones
  {
    /// <summary>
    /// Calcula las comisiones por ventas
    /// de un punto, su responsable y de una Agencia.
    /// </summary>
    /// <param name="consulta">Parametros de entrada de idCentroServicio-TipoServicio.</param>
    /// <returns>Las Comisiones del Punto y su responsable - Agencia </returns>
    CMComisionXVentaCalculadaDC CalcularComisionesxVentas(CMConsultaComisionVenta consulta);

      /// <summary>
    /// Calcula las comisiones por ventas
    /// de un punto, su responsable y de una Agencia.
    /// </summary>
    /// <param name="consulta">Parametros de entrada de idCentroServicio-TipoServicio.</param>
    /// <returns>Las Comisiones del Punto y su responsable - Agencia </returns>
    CMComisionXVentaCalculadaDC CalcularComisionesxVentas(CMConsultaComisionVenta consulta, SqlConnection conexion, SqlTransaction transaccion);

    /// <summary>
    /// Retorna el centro de servicio responsable de las comisiones del centro de servicio pasado como parámetro para el servicio dado
    /// </summary>
    /// <param name="idCentroServicio"></param>
    /// <param name="idServicio"></param>
    /// <returns></returns>
    PUCentroServiciosDC ObtenerCentroServicioResponsableComisiones(long idCentroServicio, int idServicio);

    /// <summary>
    /// Almacena las comisiones de una venta, una entrega o un pago
    /// </summary>
    /// <param name="comision">valores de la comision ganada por la agencia/punto</param>
    void GuardarComision(CMComisionXVentaCalculadaDC comision);


    /// <summary>
    /// Almacena las comisiones de una venta, una entrega o un pago
    /// </summary>
    /// <param name="comision">valores de la comision ganada por la agencia/punto</param>
    void GuardarComision(CMComisionXVentaCalculadaDC comision, SqlConnection conexion, SqlTransaction transaccion);

    /// <summary>
    /// Obtener las comisiones fijas de un centro servicios activas
    /// </summary>
    /// <param name="fechaCorte"></param>
    /// <param name="idCentroServicios"></param>
    /// <returns></returns>
    List<CMComisionesConceptosAdicionales> ObtenerComisionesFijasCentroSvcContrato(long idCentroServicios);

    /// <summary>
    /// Obtiene las comisiones del punto y del responsable
    /// por el numero de la Operacion
    /// </summary>
    /// <param name="numeroOperacion">el numero de la Operacion sea giro, Guia
    /// ó el valor guardado en RTD_Numero en la tbl RegistroTransacDetalleCaja_CAJ</param>
    /// <returns>lista de las Comisiones asociadas</returns>
    List<CMComisionXVentaCalculadaDC> ObtenerComisionPtoYCentroResponsable(long numeroOperacion);
  }
}