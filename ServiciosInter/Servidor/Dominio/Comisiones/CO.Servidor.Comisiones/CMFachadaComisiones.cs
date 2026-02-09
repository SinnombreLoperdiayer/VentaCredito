using System;
using System.Collections.Generic;
using CO.Servidor.Dominio.Comun.Comisiones;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using System.Data.SqlClient;

namespace CO.Servidor.Comisiones
{
  /// <summary>
  /// Fachada para exponer lógica de comisiones a otros modulos
  /// </summary>
  public class CMFachadaComisiones : ICMFachadaComisiones
  {
    /// <summary>
    /// Calcula las comisiones por ventas
    /// de un punto, su responsable y de una Agencia.
    /// </summary>
    /// <param name="consulta">Parametros de entrada de idCentroServicio-TipoServicio.</param>
    /// <returns>Las Comisiones del Punto y su responsable - Agencia </returns>
    public CMComisionXVentaCalculadaDC CalcularComisionesxVentas(CMConsultaComisionVenta consulta)
    {
      return CMLiquidadorComisiones.Instancia.CalcularComisionesxVentas(consulta);
    }

    /// <summary>
    /// Calcula las comisiones por ventas
    /// de un punto, su responsable y de una Agencia.
    /// </summary>
    /// <param name="consulta">Parametros de entrada de idCentroServicio-TipoServicio.</param>
    /// <returns>Las Comisiones del Punto y su responsable - Agencia </returns>
    public CMComisionXVentaCalculadaDC CalcularComisionesxVentas(CMConsultaComisionVenta consulta,SqlConnection conexion, SqlTransaction transaccion)
    {
        return CMLiquidadorComisiones.Instancia.CalcularComisionesxVentas(consulta,conexion,transaccion);
    }

    /// <summary>
    /// Retorna el centro de servicio responsable de las comisiones del centro de servicio pasado como parámetro para el servicio dado
    /// </summary>
    /// <param name="idCentroServicio"></param>
    /// <param name="idServicio"></param>
    /// <returns></returns>
    public PUCentroServiciosDC ObtenerCentroServicioResponsableComisiones(long idCentroServicio, int idServicio)
    {
      return CMLiquidadorComisiones.Instancia.ObtenerCentroServicioResponsableComisiones(idCentroServicio, idServicio);
    }

    /// <summary>
    /// Almacena las comisiones de una venta, una entrega o un pago
    /// </summary>
    /// <param name="comision">valores de la comision ganada por la agencia/punto</param>
    public void GuardarComision(CMComisionXVentaCalculadaDC comision, SqlConnection conexion, SqlTransaction transaccion)
    {
      CMLiquidadorComisiones.Instancia.GuardarComision(comision,conexion,transaccion);
    }

    /// <summary>
    /// Almacena las comisiones de una venta, una entrega o un pago
    /// </summary>
    /// <param name="comision">valores de la comision ganada por la agencia/punto</param>
    public void GuardarComision(CMComisionXVentaCalculadaDC comision)
    {
        CMLiquidadorComisiones.Instancia.GuardarComision(comision);
    }

    /// <summary>
    /// Obtener las comisiones fijas de un centro servicios activas
    /// </summary>
    /// <param name="fechaCorte"></param>
    /// <param name="idCentroServicios"></param>
    /// <returns></returns>
    public List<CMComisionesConceptosAdicionales> ObtenerComisionesFijasCentroSvcContrato(long idCentroServicios)
    {
      return CMConfiguradorComisiones.Instancia.ObtenerComisionesFijasCentroSvcContrato(idCentroServicios);
    }

    public void CalculoGuardadoComisiones(CMConsultaComisionVenta consulta)
    {
      CMComisionXVentaCalculadaDC comision = CalcularComisionesxVentas(consulta);
      GuardarComision(comision);
    }

    /// <summary>
    /// Obtiene las comisiones del punto y del responsable
    /// por el numero de la Operacion
    /// </summary>
    /// <param name="numeroOperacion">el numero de la Operacion sea giro, Guia
    /// ó el valor guardado en RTD_Numero en la tbl RegistroTransacDetalleCaja_CAJ</param>
    /// <returns>lista de las Comisiones asociadas</returns>
    public List<CMComisionXVentaCalculadaDC> ObtenerComisionPtoYCentroResponsable(long numeroOperacion)
    {
      return CMLiquidadorComisiones.Instancia.ObtenerComisionPtoYCentroResponsable(numeroOperacion);
    }
  }
}