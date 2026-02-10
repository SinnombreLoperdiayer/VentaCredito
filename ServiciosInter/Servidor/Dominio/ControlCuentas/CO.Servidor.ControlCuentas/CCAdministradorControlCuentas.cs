using System;
using System.Collections.Generic;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.ControlCuentas;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Admisiones;
using System.ServiceModel;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Comun;

namespace CO.Servidor.ControlCuentas
{
  /// <summary>
  /// Administrador para el Control de cuentas
  /// </summary>
  public class CCAdministradorControlCuentas
  {
    private static readonly CCAdministradorControlCuentas instancia = new CCAdministradorControlCuentas();

    public static CCAdministradorControlCuentas Instancia
    {
      get { return CCAdministradorControlCuentas.instancia; }
    }

    /// <summary>
    /// Obtiene el ultimo estado y ubicacin de la admision mensajeria
    /// </summary>
    /// <param name="idAdmisionMensajeria"></param>
    public ADGuiaUltEstadoDC ObtenerMensajeriaUltimoEstado(long idNumeroGuia)
    {
      return CCControlCuentas.Instancia.ObtenerMensajeriaUltimoEstado(idNumeroGuia);
    }

    /// <summary>
    /// Obtener informacion de la guia de mensajeria y las formas de pago
    /// </summary>
    /// <returns></returns>
    public ADGuiaUltEstadoDC ObtenerMensajeriaFormaPago(long idAdmision)
    {
      return CCControlCuentas.Instancia.ObtenerMensajeriaFormaPago(idAdmision);
    }

    /// <summary>
    /// Calcula el precio de un guia para cambio de destino
    /// </summary>
    public decimal ReLiquidacion(ADGuia guia)
    {
      return CCControlCuentas.Instancia.ReLiquidacion(guia);
    }

    /// <summary>
    /// Calcula el precio de un guia para cambio de destino
    /// </summary>
    public CCBolsaNovedadesReliquidacionDC ReLiquidacionBolsaNovedades(ADGuia guia)
    {
        return CCControlCuentas.Instancia.ReLiquidacionBolsaNovedades(guia);
    }
    /// <summary>
    /// Recalcula la prima de un factura
    /// </summary>
    public decimal ReLiquidacionPrima(ADGuia guia)
    {
        return CCControlCuentas.Instancia.ReLiquidacionPrima(guia);
    }

    /// <summary>
    /// Obtener el empleado en NovaSoft
    /// </summary>
    /// <param name="identificacion"></param>
    public CCEmpleadoNovaSoftDC ObtenerEmpleadoNovaSoft(string identificacion)
    {
      return CCControlCuentas.Instancia.ObtenerEmpleadoNovaSoft(identificacion);
    }

    /// <summary>
    /// Crear novedad cambio de destino
    /// </summary>
    /// <param name="novedadGuia"></param>
    public void CrearNovedadCambioDestino(CCNovedadCambioDestinoDC novedadGuia)
    {
      CCControlCuentas.Instancia.CrearNovedadCambioDestino(novedadGuia);
    }

    /// <summary>
    /// Crear novedad cambio de peso
    /// </summary>
    /// <param name="novedadGuia"></param>
    public void CrearNovedadCambioPeso(CCNovedadCambioPesoDC novedadGuia)
    {
        CCControlCuentas.Instancia.CrearNovedadCambioPeso(novedadGuia);
    }

    /// <summary>
    /// crear novedad cambio forma de pago
    /// </summary>
    /// <param name="novedadGuia"></param>
    public void CrearNovedadFormaPago(CCNovedadCambioFormaPagoDC novedadGuia)
    {
      CCControlCuentas.Instancia.CrearNovedadFormaPago(novedadGuia);
    }    

    /// <summary>
    /// Actualiza en la base de datos los datos del remitente y del destinatario de una guía o factura
    /// </summary>
    /// <param name="novedadGuia"></param>
    public void ActualizarRemitenteDestinatarioGuia(CCNovedadCambioRemitenteDC novedadGuia)
    {
      CCControlCuentas.Instancia.ActualizarRemitenteDestinatarioGuia(novedadGuia);
    }

    /// <summary>
    /// Crea novedad de cambio de tipo de servicio en una admisión
    /// </summary>
    /// <param name="novedadGuia"></param>
    public void CrearNovedadCambioTipoServicio(CCNovedadCambioTipoServicio novedadGuia)
    {
      CCControlCuentas.Instancia.CrearNovedadCambioTipoServicio(novedadGuia);
    }

    /// <summary>
    /// Obtene los datos del mensajero de la agencia.
    /// </summary>
    /// <param name="idAgencia">Es el id agencia.</param>
    /// <returns>la lista de mensajeros de una agencia</returns>
    public IEnumerable<OUNombresMensajeroDC> ObtenerNombreMensajeroAgencia(long idAgencia)
    {
      return CCControlCuentas.Instancia.ObtenerNombreMensajeroAgencia(idAgencia);
    }

    /// <summary>
    /// Obtiene los clientes y sus contratos por agencia
    /// </summary>
    /// <param name="idAgencia">Identificador Agencia</param>
    /// <returns>Colección clientes y contratos</returns>
    public List<CLClientesDC> ObtenerClientesContratosXAgencia(long idAgencia)
    {
      return CCControlCuentas.Instancia.ObtenerClientesContratosXAgencia(idAgencia);
    }

        /// <summary>
    /// Trae el listado de los clientes crédito y sus respectivos contratos por una agencia y todas las agencias dependientes de la misma
    /// </summary>
    /// <param name="idAgencia"></param>
    /// <returns></returns>
    public List<CLClientesDC> ObtenerCLientesContratosXAgenciaDependientes(long idAgencia)
    {
      return CCControlCuentas.Instancia.ObtenerCLientesContratosXAgenciaDependientes(idAgencia);
    }

    /// <summary>
    /// Obtiene los puntosde atencion
    /// de una agencia centro Servicio.
    /// </summary>
    /// <param name="idCentroServicio">The id centro servicio.</param>
    /// <returns>Lista de Puntos de Una Agencia</returns>
    public List<PUCentroServiciosDC> ObtenerPuntosDeAgencia(long idCentroServicio)
    {
      return CCControlCuentas.Instancia.ObtenerPuntosDeAgencia(idCentroServicio);
    }

    /// <summary>
    /// Retorna la lista de puntos y agencias dependientes de un centro de servicio
    /// </summary>
    /// <param name="idCentroServicio"></param>
    /// <returns></returns>
    public List<PUCentroServiciosDC> ObtenerPuntosAgenciasDependientes(long idCentroServicio)
    {
      return CCControlCuentas.Instancia.ObtenerPuntosAgenciasDependientes(idCentroServicio);
    }

    /// <summary>
    /// Obtiene las agencias de la aplicación sin filtro
    /// </summary>
    public List<PUCentroServiciosDC> ObtenerAgencias()
    {
      return CCControlCuentas.Instancia.ObtenerAgencias();
    }


    /// <summary>
    /// Obtiene las guías de mensajería a partir de una agencia
    /// </summary>
    /// <param name="agencia">Objeto Agencia</param>
    /// <returns>Colección de guías</returns>
    public List<ADGuia> ObtenerGuiasAgencia(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal)
    {
      return CCControlCuentas.Instancia.ObtenerGuiasAgencia(agencia, fechaInicial, fechaFinal);
    }

    /// <summary>
    /// Obtiene las guías de mensajería a partir de una agencia y un cliente
    /// </summary>
    /// <param name="agencia">Objeto Agencia</param>
    /// <param name="cliente">Objeto Cliente</param>
    /// <returns>Colección guías de mensajería</returns>
    public List<ADGuia> ObtenerGuiasClienteCredito(PUCentroServiciosDC agencia, CLClientesDC cliente, DateTime fechaInicial, DateTime fechaFinal, int idSucursal)
    {
      return CCControlCuentas.Instancia.ObtenerGuiasClienteCredito(agencia, cliente, fechaInicial, fechaFinal, idSucursal);
    }

    /// <summary>
    /// Obtiene las guías de mensajería a partir de una agencia y un mensajero
    /// </summary>
    /// <param name="agencia">Objeto Agencia</param>
    /// <param name="cliente">Objeto Cliente</param>
    /// <returns>Colección guías de mensajería</returns>
    public List<ADGuia> ObtenerGuiasMensajero(PUCentroServiciosDC agencia, OUNombresMensajeroDC mensajero, DateTime fechaInicial, DateTime fechaFinal)
    {
      return CCControlCuentas.Instancia.ObtenerGuiasMensajero(agencia, mensajero, fechaInicial, fechaFinal);
    }

    /// <summary>
    /// Adiciona un registro al almacen de control de cuentas
    /// </summary>
    /// <param name="almacen">Objeto almacen</param>
    public CCAlmacenDC AdicionarAlmacenControlCuentas(CCAlmacenDC almacen)
    {
      return CCControlCuentas.Instancia.AdicionarAlmacenControlCuentas(almacen);
    }

    /// <summary>
    /// Adiciona al almacén de control de cuentas una guía anulada
    /// </summary>
    /// <param name="almacen"></param>
    /// <returns></returns>
    public CCAlmacenDC AdicionarAlmacenGuiaAnulada(CCAlmacenDC almacen)
    {
        IADFachadaAdmisionesMensajeria fachadaMensajeria  = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>(); ;
        ADGuiaUltEstadoDC ultimoEstadoGuia= fachadaMensajeria.ObtenerMensajeriaUltimoEstado(almacen.IdOperacion);

        if(ultimoEstadoGuia.TrazaGuia.IdEstadoGuia==(int)ADEnumEstadoGuia.Anulada)
            return AdicionarAlmacenControlCuentas(almacen);
        else
            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_CONTROL_CUENTAS, "0", "La guía que intenta archivar no está anulada."));
    }

    /// <summary>
    /// Adiciona un registro al almacen de control de cuentas sin archivar, es decir que no adicionar lote, posición ni caja
    /// </summary>
    /// <param name="almacen">Objeto almacen</param>
    public CCAlmacenDC AdicionarAlmacenControlCuentasSinArchivar(CCAlmacenDC almacen)
    {
      return CCControlCuentas.Instancia.AdicionarAlmacenControlCuentasSinArchivar(almacen);
    }

    /// <summary>
    /// Adicionar varios registros de almacén de control de cuentas sin archivar.
    /// </summary>
    /// <param name="almacen"></param>
    /// <param name="operaciones"></param>
    public void AdicionarVariosAlmacenControlCuentasSinArchivar(CCAlmacenDC almacen, List<long> operaciones)
    {
      CCControlCuentas.Instancia.AdicionarVariosAlmacenControlCuentasSinArchivar(almacen, operaciones);
    }

    /// <summary>
    /// Obtiene los giros de una agencia
    /// </summary>
    /// <returns>Colección giros</returns>
    public List<GIAdmisionGirosDC> ObtenerGirosAgencia(PUCentroServiciosDC agencia, DateTime fechaDesde, DateTime fechaHasta)
    {
      return CCControlCuentas.Instancia.ObtenerGirosAgencia(agencia, fechaDesde, fechaHasta);
    }

    /// <summary>
    /// Obtiene las admisiones y pagos de una agencia
    /// </summary>
    /// <param name="agencia">Objeto agencia</param>
    /// <returns>Colección admisiones pagos</returns>
    public List<PGPagoAdmisionGiroDC> ObtenerAdmisionPagoGiroAgencia(PUCentroServiciosDC agencia, DateTime fechaDesde, DateTime fechaHasta)
    {
      return CCControlCuentas.Instancia.ObtenerAdmisionPagoGiroAgencia(agencia, fechaDesde, fechaHasta);
    }

    /// <summary>
    /// Obtiene los gastos de caja
    /// </summary>
    /// <param name="agencia">Objeto agencia</param>
    /// <returns>Colección de gastos</returns>
    public List<CAMovimientoCajaDC> ObtenerGastosCaja(PUCentroServiciosDC agencia, DateTime fechaDesde, DateTime fechaHasta)
    {
      return CCControlCuentas.Instancia.ObtenerGastosCaja(agencia, fechaDesde, fechaHasta);
    }

    /// <summary>
    /// Obtiene los movimientos de caja para un centro de servicio dado en un rago de fechas que difieren de ventas de mensajería, 
    /// pago de al cobros, venta de giros, pago de giros y ventas de pines prepago.
    /// </summary>
    ///<param name="agencia"></param>
    ///<param name="fechaFinal"></param>
    ///<param name="fechaInicial"></param>
    public List<CAMovimientoCajaDC> ObtenerOtrosMovimientosCaja(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal)
    {
      return CCControlCuentas.Instancia.ObtenerOtrosMovimientosCaja(agencia, fechaInicial, fechaFinal);
    }

    /// <summary>
    /// Retorna las ventas de pin prepago realizads por la gencia en el rango de fechas dado
    /// </summary>
    /// <param name="agencia"></param>
    /// <param name="fechaInicial"></param>
    /// <param name="fechaFinal"></param>
    /// <returns></returns>
    public List<CAMovimientoCajaDC> ObtenerVentasPinPrepago(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal)
    {
      return CCControlCuentas.Instancia.ObtenerVentasPinPrepago(agencia, fechaInicial, fechaFinal);
    }

    /// <summary>
    /// Retorna las operaciones de caja que tengan el concepto de pago "Al Cobro" realizadas por la agencia en el rango de fechas dado
    /// </summary>
    /// <param name="agencia"></param>
    /// <param name="fechaInicial"></param>
    /// <param name="fechaFinal"></param>
    /// <returns></returns>
    public List<CAMovimientoCajaDC> ObtenerRecaudosAlCobro(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal)
    {
      return CCControlCuentas.Instancia.ObtenerRecaudosAlCobro(agencia, fechaInicial, fechaFinal);
    }

    /// <summary>
    /// Obtiene los motivos de anulación de una guía
    /// </summary>
    /// <returns>Colección motivos</returns>
    public List<ADMotivoAnulacionDC> ObtenerMotivosAnulacion()
    {
      return CCControlCuentas.Instancia.ObtenerMotivosAnulacion();
    }

    /// <summary>
    /// Anula una guía de mensajería
    /// </summary>
    /// <param name="anulacion">Objeto</param>
    public CCResultadoAnulacionGuia AnularGuia(CCAnulacionGuiaMensajeriaDC anulacion)
    {
      return CCControlCuentas.Instancia.AnularGuia(anulacion);
    }

    /// <summary>
    /// Adiciona una guía anulada. Se usa para la parte de anulación de una guía. Se espera uqe se pase el id del centro de servicio de origen y el número de la guía.
    /// </summary>
    /// <param name="guia"></param>
    public long AdicionarAdmisionAnulada(CCAnulacionGuiaMensajeriaDC guia)
    {
      return CCControlCuentas.Instancia.AdicionarAdmisionAnulada(guia);
    }

    /// <summary>
    /// Crea novedad de cambio de valor total de una admisión
    /// </summary>
    /// <param name="novedadGuia"></param>
    public void CrearNovedadCambioValorTotal(CCNovedadCambioValorTotal novedadGuia)
    {
      CCControlCuentas.Instancia.CrearNovedadCambioValorTotal(novedadGuia);
    }
  }
}