using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Comisiones.Datos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System.Data.SqlClient;

namespace CO.Servidor.Comisiones
{
  internal class CMLiquidadorComisiones : ControllerBase
  {
    private static readonly CMLiquidadorComisiones instancia = (CMLiquidadorComisiones)FabricaInterceptores.GetProxy(new CMLiquidadorComisiones(), COConstantesModulos.COMISIONES);

    public static CMLiquidadorComisiones Instancia
    {
      get { return CMLiquidadorComisiones.instancia; }
    }

    /// <summary>
    /// Calcula las comisiones por ventas
    /// de un punto, su responsable y de una Agencia.
    /// </summary>
    /// <param name="consulta">Parametros de entrada de idCentroServicio-TipoServicio.</param>
    /// <returns>Las Comisiones del Punto y su responsable - Agencia </returns>
    public CMComisionXVentaCalculadaDC CalcularComisionesxVentas(CMConsultaComisionVenta consulta)
    {
      return CMRepositorio.Instancia.CalcularComisionesxVentas(consulta);
    }

    /// <summary>
    /// Calcula las comisiones por ventas
    /// de un punto, su responsable y de una Agencia.
    /// </summary>
    /// <param name="consulta">Parametros de entrada de idCentroServicio-TipoServicio.</param>
    /// <returns>Las Comisiones del Punto y su responsable - Agencia </returns>
    public CMComisionXVentaCalculadaDC CalcularComisionesxVentas(CMConsultaComisionVenta consulta,SqlConnection conexion, SqlTransaction transaccion)
    {
        return CMRepositorio.Instancia.CalcularComisionesxVentas(consulta,conexion,transaccion);
    }


    /// <summary>
    /// Almacena las comisiones de una venta, una entrega o un pago
    /// </summary>
    /// <param name="comision">valores de la comision ganada por la agencia/punto</param>
    public void GuardarComision(CMComisionXVentaCalculadaDC comision)
    {
      CMRepositorio.Instancia.GuardarComision(comision);
    }
    
      /// <summary>
    /// Almacena las comisiones de una venta, una entrega o un pago
    /// </summary>
    /// <param name="comision">valores de la comision ganada por la agencia/punto</param>
    public void GuardarComision(CMComisionXVentaCalculadaDC comision, SqlConnection conexion, SqlTransaction transaccion)
    {
      CMRepositorio.Instancia.GuardarComision(comision,conexion,transaccion);
    }

    /// <summary>
    /// Retorna el centro de servicio responsable de las comisiones del centro de servicio pasado como parámetro para el servicio dado
    /// </summary>
    /// <param name="idCentroServicio"></param>
    /// <param name="idServicio"></param>
    /// <returns></returns>
    public PUCentroServiciosDC ObtenerCentroServicioResponsableComisiones(long idCentroServicio, int idServicio)
    {
      return CMRepositorio.Instancia.ObtenerCentroServicioResponsableComisiones(idCentroServicio, idServicio);
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
      return CMRepositorio.Instancia.ObtenerComisionPtoYCentroResponsable(numeroOperacion);
    }
  }
}