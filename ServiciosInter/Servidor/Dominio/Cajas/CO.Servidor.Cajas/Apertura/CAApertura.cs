using System;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using CO.Servidor.Cajas.CajaFinanciera;
using CO.Servidor.Cajas.CajaVenta;
using CO.Servidor.Cajas.Comun;
using CO.Servidor.Cajas.Datos;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using System.Data.SqlClient;

namespace CO.Servidor.Cajas.Apertura
{
  /// <summary>
  /// Clase para el manejo de la apertura de caja
  /// </summary>
  internal partial class CAApertura : ControllerBase
  {
    #region campos

    private CATransaccionCaja registroTransaccion;
    private CABanco cajaBanco;
    private CACajaCasaMatriz cajaCasaMatriz;
    private CACajaOperacionNacional cajaOperacionNacional;

    #endregion campos

    #region Atributos

    private static CAApertura instancia = (CAApertura)FabricaInterceptores.GetProxy(new CAApertura(), COConstantesModulos.CAJA);

    #endregion Atributos

    #region Instancia

    public static CAApertura Instancia
    {
      get
      {
        return CAApertura.instancia;
      }
    }

    #endregion Instancia

    #region ctor

    private CAApertura()
    {
      registroTransaccion = new CATransaccionCaja();
      cajaBanco = new CABanco();
      cajaOperacionNacional = new CACajaOperacionNacional();
      cajaCasaMatriz = new CACajaCasaMatriz();
    }

    #endregion ctor

    /// <summary>
    /// Validar la Apertura de la Caja de Casa Matriz, si no esta abierta la abre, si tiene una caja abierta con otro centro de servicio lanza error.
    /// </summary>
    /// <param name="infoCaja">Objeto con información relacionada con la caja.</param>
    /// <param name="idCasaMatriz">Identificador de la casa matriz sobre la cual se hace la operación</param>
    /// <param name="tipoApertura">Tipo de apertura de caja</param>
    /// <returns>Identificador del registro de apertura</returns>
    internal long ValidarAperturaCajaGestion(CAAperturaCajaDC infoCaja, short idCasaMatriz, CAEnumTipoApertura tipoApertura)
    {
      // Sólo se permiten aperturas sobre la caja desde una casa matriz en la caja cero (0)
      if (infoCaja.IdCaja != 0)
      {
        throw new FaultException<ControllerException>(
          new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja.ERROR_NO_AUTORIZADO.ToString(),
           CACajaServerMensajes.CargarMensaje(CAEnumTipoErrorCaja.ERROR_NO_AUTORIZADO))
       );
      }

      // validar apertura en la casa matriz
      infoCaja.IdAperturaCaja = CARepositorioCaja.Instancia.ValidarApeturaCajaGestion(infoCaja.IdCodigoUsuario, idCasaMatriz, tipoApertura);

      //si no existe apertura se crea el registro en tabla apertura Caja y AperturaCasaMatriz
      if (infoCaja.IdAperturaCaja == 0)
      {
        // crear registro de caja en la base de datos
        infoCaja.IdAperturaCaja = CARepositorioCaja.Instancia.AdicionarAperturaCaja(infoCaja, tipoApertura);
        CARepositorioCaja.Instancia.AdicionarAperturaCajaCasaMatriz(infoCaja, idCasaMatriz);
      }

      return infoCaja.IdAperturaCaja;
    }

    /// <summary>
    /// Valida la Apertura de la Caja de Centro Servicio, si no esta abierta la abre, si tiene una caja abierta con otro centro de servicio lanza error
    /// </summary>
    /// <param name="infoCaja">Objeto con información relacionada con la caja</param>
    /// <param name="idCentroServicio">Identificador del centro de servicios desde el cual se hace la operación</param>
    /// <param name="idCentroServiciosValidarApertura">Identificador del centro de servicios con el cual se desea validar la apertura</param>
    /// <returns>Identificador del registro de apertura</returns>
    /// <remarks>Se usa el centro de servicios validar apertura para el caso de movimientos de caja que afectan una caja de otro centro de servicios</remarks>
    internal long ValidarAperturaCajaCentroServicios(CAAperturaCajaDC infoCaja, long idCentroServicio, long idCentroServiciosValidarApertura = 0)
    {
      // escoger sobre cual centro de servicios se hace la validación de la apertura
      long idCentroSvcValidar = idCentroServiciosValidarApertura == 0 ? idCentroServicio : idCentroServiciosValidarApertura;

      //cargar la base inicial actualmente configurada
      IPUFachadaCentroServicios centroSvc = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
      PUCentroServiciosDC centroSvcDC = centroSvc.ObtenerCentroServicio(idCentroSvcValidar);
      infoCaja.BaseInicialApertura = centroSvcDC.BaseInicialCaja;

      //Validar apertura
      infoCaja.IdAperturaCaja = CARepositorioCaja.Instancia.ValidarApeturaCentroServicios(infoCaja.IdCaja,
                                                                                          infoCaja.IdCodigoUsuario,
                                                                                          idCentroServicio);

      //si no existe apertura se crea el registro en tabla apertura Caja y AperturaCentroServicio
      if (infoCaja.IdAperturaCaja == 0)
      {
        //crear registro de apertura en la base de datos
        infoCaja.IdAperturaCaja = CARepositorioCaja.Instancia.AdicionarAperturaCaja(infoCaja, CAEnumTipoApertura.CES);
        CARepositorioCaja.Instancia.AdicionarAperturaCentroServicios(infoCaja, idCentroServicio, centroSvcDC.Nombre);
      }

      //marcar caja como abierta (o no abierta)
      infoCaja.EstaAbierta = infoCaja.IdAperturaCaja != 0;

      return infoCaja.IdAperturaCaja;
    }


    /// <summary>
    /// Valida la Apertura de la Caja de Centro Servicio, si no esta abierta la abre, si tiene una caja abierta con otro centro de servicio lanza error
    /// </summary>
    /// <param name="infoCaja">Objeto con información relacionada con la caja</param>
    /// <param name="idCentroServicio">Identificador del centro de servicios desde el cual se hace la operación</param>
    /// <param name="idCentroServiciosValidarApertura">Identificador del centro de servicios con el cual se desea validar la apertura</param>
    /// <returns>Identificador del registro de apertura</returns>
    /// <remarks>Se usa el centro de servicios validar apertura para el caso de movimientos de caja que afectan una caja de otro centro de servicios</remarks>
    internal long ValidarAperturaCajaCentroServicios(CAAperturaCajaDC infoCaja, long idCentroServicio,SqlConnection conexion, SqlTransaction transaccion, long idCentroServiciosValidarApertura = 0)
    {
        // escoger sobre cual centro de servicios se hace la validación de la apertura
        long idCentroSvcValidar = idCentroServiciosValidarApertura == 0 ? idCentroServicio : idCentroServiciosValidarApertura;

        //cargar la base inicial actualmente configurada
        IPUFachadaCentroServicios centroSvc = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
        PUCentroServiciosDC centroSvcDC = centroSvc.ObtenerCentroServicio(idCentroSvcValidar);
        infoCaja.BaseInicialApertura = centroSvcDC.BaseInicialCaja;

        //Validar apertura
        infoCaja.IdAperturaCaja = CARepositorioCaja.Instancia.ValidarApeturaCentroServicios(infoCaja.IdCaja,
                                                                                            infoCaja.IdCodigoUsuario,
                                                                                            idCentroServicio,conexion,transaccion);

        //si no existe apertura se crea el registro en tabla apertura Caja y AperturaCentroServicio
        if (infoCaja.IdAperturaCaja == 0)
        {
            //crear registro de apertura en la base de datos
            infoCaja.IdAperturaCaja = CARepositorioCaja.Instancia.AdicionarAperturaCaja(infoCaja, CAEnumTipoApertura.CES,conexion,transaccion);
            CARepositorioCaja.Instancia.AdicionarAperturaCentroServicios(infoCaja, idCentroServicio, centroSvcDC.Nombre,conexion,transaccion);
        }

        //marcar caja como abierta (o no abierta)
        infoCaja.EstaAbierta = infoCaja.IdAperturaCaja != 0;

        return infoCaja.IdAperturaCaja;
    }


    /// <summary>
    /// Registra la Venta en la caja, valida la caja Abierta este método está en una Transaccion.
    /// </summary>
    /// <param name="infoTransaccion">Objeto con la información de la transacción.</param>
    /// <param name="idCentroServiciosValidarApertura">Identificador del centro de servicios con el cual se desea validar la apertura</param>
    /// <param name="anulacionGuia">Indica si el movimiento es para anulacion de guia</param>
    /// <returns>Identificador del registro de la venta</returns>
    /// <remarks>Se usa el centro de servicios validar apertura para el caso de movimientos de caja que afectan una caja de otro centro de servicios</remarks>
    internal CAIdTransaccionesCajaDC RegistrarVenta(CARegistroTransacCajaDC infoTransaccion, long idCentroServiciosValidarApertura = 0)
    {
      infoTransaccion.InfoAperturaCaja.IdAperturaCaja = ValidarAperturaCajaCentroServicios(infoTransaccion.InfoAperturaCaja, infoTransaccion.IdCentroServiciosVenta, idCentroServiciosValidarApertura);

      return registroTransaccion.AdicionarMovimientoCaja(infoTransaccion);
    }
    /// <summary>
    /// Registra la Venta en la caja, valida la caja Abierta este método está en una Transaccion.
    /// </summary>
    /// <param name="infoTransaccion">Objeto con la información de la transacción.</param>
    /// <param name="idCentroServiciosValidarApertura">Identificador del centro de servicios con el cual se desea validar la apertura</param>
    /// <param name="anulacionGuia">Indica si el movimiento es para anulacion de guia</param>
    /// <returns>Identificador del registro de la venta</returns>
    /// <remarks>Se usa el centro de servicios validar apertura para el caso de movimientos de caja que afectan una caja de otro centro de servicios</remarks>
    internal CAIdTransaccionesCajaDC RegistrarVenta(CARegistroTransacCajaDC infoTransaccion, SqlConnection conexion,SqlTransaction transaccion, long idCentroServiciosValidarApertura = 0)
    {
        infoTransaccion.InfoAperturaCaja.IdAperturaCaja = ValidarAperturaCajaCentroServicios(infoTransaccion.InfoAperturaCaja, infoTransaccion.IdCentroServiciosVenta,conexion,transaccion, idCentroServiciosValidarApertura);

        return registroTransaccion.AdicionarMovimientoCaja(infoTransaccion,conexion,transaccion);
    }

    /// <summary>
    /// Registra la Venta en la caja, valida la caja Abierta este método está en una Transaccion.
    /// </summary>
    /// <param name="infoTransaccion">Objeto con la información de la transacción.</param>
    /// <param name="idCentroServiciosValidarApertura">Identificador del centro de servicios con el cual se desea validar la apertura</param>
    /// <param name="anulacionGuia">Indica si el movimiento es para anulacion de guia</param>
    /// <returns>Identificador del registro de la venta</returns>
    /// <remarks>Se usa el centro de servicios validar apertura para el caso de movimientos de caja que afectan una caja de otro centro de servicios</remarks>
    internal CAIdTransaccionesCajaDC RegistrarAnulacionGuia(CARegistroTransacCajaDC infoTransaccion, bool FormaPagoAlCobro)
    {
      infoTransaccion.InfoAperturaCaja.IdAperturaCaja = ValidarAperturaCajaCentroServicios(infoTransaccion.InfoAperturaCaja, infoTransaccion.IdCentroServiciosVenta);
      ///Si la forma de pago es alCobro se valida que este abierta la caja del centro se servicios destino de la guia
      if (FormaPagoAlCobro)
        infoTransaccion.InfoAperturaCaja.IdAperturaCaja = ValidarAperturaCajaCentroServicios(new CAAperturaCajaDC() { IdCaja = 0, IdCodigoUsuario = infoTransaccion.InfoAperturaCaja.IdCodigoUsuario }, infoTransaccion.IdCentroServiciosDestinoGuia);

      return registroTransaccion.AdicionarMovimientoCaja(infoTransaccion, true);
    }

    internal CAIdTransaccionesCajaDC RegistrarVentaConNuevaApertura(CARegistroTransacCajaDC infoTransaccion)
    {
      //infoTransaccion.InfoAperturaCaja.IdAperturaCaja = ValidarAperturaCajaCentroServicios(infoTransaccion.InfoAperturaCaja, infoTransaccion.IdCentroServiciosVenta, idCentroServiciosValidarApertura);

      //crear registro de apertura en la base de datos
      infoTransaccion.InfoAperturaCaja.IdCaja = 0;
      infoTransaccion.InfoAperturaCaja.IdAperturaCaja = CARepositorioCaja.Instancia.AdicionarAperturaCaja(infoTransaccion.InfoAperturaCaja, CAEnumTipoApertura.CES);
      CARepositorioCaja.Instancia.AdicionarAperturaCentroServicios(infoTransaccion.InfoAperturaCaja, infoTransaccion.IdCentroServiciosVenta, infoTransaccion.NombreCentroServiciosVenta);

      return registroTransaccion.AdicionarMovimientoCaja(infoTransaccion);
    }

    /// <summary>
    /// Registra la Venta en la caja, valida la caja abieta.
    /// </summary>
    /// <param name="infoTransaccion">Objeto con la información de la transacción.</param>
    /// <param name="idCentroServiciosValidarApertura">Identificador del centro de servicios con el cual se desea validar la apertura</param>
    /// <returns>Identificador del registro de la venta</returns>
    /// <remarks>Se usa el centro de servicios validar apertura para el caso de movimientos de caja que afectan una caja de otro centro de servicios</remarks>
    internal CAIdTransaccionesCajaDC RegistrarVentaRequiereTipoComprobante(CARegistroTransacCajaDC infoTransaccion, PAEnumConsecutivos? idConsecutivoComprobante, long idCentroServiciosValidarApertura = 0)
    {
      infoTransaccion.InfoAperturaCaja.IdAperturaCaja = ValidarAperturaCajaCentroServicios(infoTransaccion.InfoAperturaCaja, infoTransaccion.IdCentroServiciosVenta, idCentroServiciosValidarApertura);

      if (!idConsecutivoComprobante.HasValue)
      {
        return registroTransaccion.AdicionarMovimientoCaja(infoTransaccion);
      }
      else
      {
        return registroTransaccion.AdicionarMovimientoCaja(infoTransaccion, idConsecutivoComprobante.Value);
      }
    }

    /// <summary>
    /// Registra la Venta en la caja, valida la caja Abieta
    /// este metodo esta en una Transaccion.
    /// </summary>
    /// <param name="infoTransaccion">The info transaccion.</param>
    /// <param name="idCentroServiciosValidarApertura">The id centro servicios validar apertura.</param>
    /// <returns>el registro de la transaccion</returns>
    internal CAIdTransaccionesCajaDC RegistrarVentaTransaccional(CARegistroTransacCajaDC infoTransaccion, long idCentroServiciosValidarApertura = 0)
    {
      CAIdTransaccionesCajaDC transaccion = null;

      using (TransactionScope trans = new TransactionScope())
      {
        transaccion = RegistrarVenta(infoTransaccion, idCentroServiciosValidarApertura);
        trans.Complete();
      }
      return transaccion;
    }

    /// <summary>
    /// Inserta in registro del cliente credito y cambia el estado de la facturacion y el tipo de dato
    /// Este metodo se utiliza para el cambio de forma de pago de AlCobro -Credito
    /// </summary>
    internal void AdicionarRegistroTransClienteCredito(ADNovedadGuiaDC novedadGuia)
    {
      registroTransaccion.AdicionarRegistroTransClienteCredito(novedadGuia);
    }

    internal long RegistrarOperacionCasaMatriz(CACajaCasaMatrizDC infoTransaccion)
    {
      long idApertura;

      //validar apertura
      CAAperturaCajaDC apertura = new CAAperturaCajaDC
      {
        IdCaja = 0,
        IdCodigoUsuario = infoTransaccion.IdCodigoUsuario,
        NombresUsuario = infoTransaccion.CreadoPor,
        BaseInicialApertura = 0
      };

      idApertura = ValidarAperturaCajaGestion(apertura, (short)infoTransaccion.IdCentroServicioRegistra, CAEnumTipoApertura.CAM);

      return cajaCasaMatriz.RegistrarOperacion(idApertura, infoTransaccion);
    }

    /// <summary>
    /// Obtiene la Ultima apertura activa del usuario por Centro de
    /// servicio
    /// </summary>
    /// <param name="idCentroServicio"></param>
    /// <param name="idCaja"></param>
    /// <param name="idCodUsuario"></param>
    /// <returns>el id long de la ultima apertura</returns>
    public long ObtenerUltimaAperturaActiva(long idCentroServicio, int idCaja, long idCodUsuario)
    {
      return CARepositorioCaja.Instancia.ObtenerUltimaAperturaActiva(idCentroServicio, idCaja, idCodUsuario);
    }

    #region Operacion Caja OPN

    /// <summary>
    /// Registrar operación sobre la caja de Operación Nacional
    /// </summary>
    /// <param name="infoCaja">Información de la caja y su apertura</param>
    /// <param name="infoTransaccion">Información de la transacción a aplicar</param>
    /// <returns>Identificador con el cual que da registrada la tarnsacción en la caja de Operación Nacional</returns>
    internal long RegistraOperacionCajaOpn(CARegistroOperacionOpnDC infoTransaccion)
    {
      CAAperturaCajaDC apertura = new CAAperturaCajaDC
      {
        BaseInicialApertura = 0,
        CreadoPor = infoTransaccion.CreadoPor,
        IdCaja = 0,
        IdCodigoUsuario = infoTransaccion.IdCodigoUsuario,
      };

      return RegistraOperacionCajaOpn(apertura, infoTransaccion);
    }

    internal long RegistraOperacionCajaOpn(CAAperturaCajaDC infoCaja, CARegistroOperacionOpnDC infoTransaccion)
    {
      long idApertura = ValidarAperturaCajaGestion(infoCaja, infoTransaccion.IdCasaMatriz, CAEnumTipoApertura.OPN);

      return CajaOperacionNacional.RegistrarOperacion(idApertura, infoTransaccion);
    }

    /// <summary>
    /// Registrar operación sobre la caja de Banco
    /// </summary>
    /// <param name="infoTransaccion">Información de la caja y su apertura</param>
    /// <returns>Identificador con el cual que da registrada la tarnsacción en la caja de Banco</returns>
    internal long RegistrarOperacionBanco(CACajaBancoDC infoTransaccion)
    {
      //validar apertura
      CAAperturaCajaDC apertura = new CAAperturaCajaDC
      {
        IdCaja = 0,
        IdCodigoUsuario = infoTransaccion.IdCodigoUsuario,
        CreadoPor = infoTransaccion.CreadoPor,
        BaseInicialApertura = 0
      };

      return RegistrarOperacionBanco(apertura, infoTransaccion);
    }

    internal long RegistrarOperacionBanco(CAAperturaCajaDC infoApertura, CACajaBancoDC infoTransaccion)
    {
      infoTransaccion.IdAperturaCaja = ValidarAperturaCajaGestion(infoApertura, infoTransaccion.IdCasaMatriz, CAEnumTipoApertura.BAN);

      return cajaBanco.RegistrarOperacionBanco(infoTransaccion.IdAperturaCaja, infoTransaccion);
    }

    #endregion Operacion Caja OPN

    #region Privados

    internal CATransaccionCaja CARegistroTransaccion
    {
      get
      {
        return registroTransaccion;
      }
    }

    internal CABanco CajaBanco
    {
      get
      {
        return cajaBanco;
      }
    }

    internal CACajaCasaMatriz CajaCasaMatriz
    {
      get
      {
        return cajaCasaMatriz;
      }
    }

    internal CACajaOperacionNacional CajaOperacionNacional
    {
      get
      {
        return cajaOperacionNacional;
      }
    }

    #endregion Privados
  }
}