using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using CO.Controller.Servidor.Integraciones.CuatroSieteDos;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.GestionGiros.Comun;
using CO.Servidor.PagosManuales.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.GestionGiros.PagosManuales;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;

namespace CO.Servidor.PagosManuales
{
  public class GIPagosManuales : ControllerBase
  {
    #region CrearInstancia

    private static readonly GIPagosManuales instancia = (GIPagosManuales)FabricaInterceptores.GetProxy(new GIPagosManuales(), COConstantesModulos.GIROS);

    /// <summary>
    /// Retorna una instancia de centro Servicios
    /// /// </summary>
    public static GIPagosManuales Instancia
    {
      get { return GIPagosManuales.instancia; }
    }

    #endregion CrearInstancia

    /// <summary>
    /// consulta los giros activos realizados el dia actual peaton peaton
    /// </summary>
    /// <param name="idCentroServicio"></param>
    /// <returns></returns>
    public List<GIAdmisionGirosDC> ConsultarGirosPeatonPeatonPorAgencia(long idCentroServicioOrigen, int indicePagina, int registrosPorPagina)
    {
      return GIRepositorioPagosManuales.Instancia.ConsultarGirosPeatonPeatonPorAgencia(idCentroServicioOrigen, indicePagina, registrosPorPagina);
    }

    /// <summary>
    /// consulta los giros activos realizados el dia actual peaton peaton
    /// </summary>
    /// <param name="idCentroServicio"></param>
    /// <returns></returns>
    public List<GIAdmisionGirosDC> ConsultarGirosPeatonPeatonPorAgenciaPorTrasmitir(long idCentroServicioDestino)
    {
      return GIRepositorioPagosManuales.Instancia.ConsultarGirosPeatonPeatonPorAgenciaPorTrasmitir(idCentroServicioDestino);
    }

    /// <summary>
    /// Consultar informacion del giro adicionales - impuestos- intentos transmitir
    /// </summary>
    /// <param name="idGiro"></param>
    public GIAdmisionGirosDC ConsultarGiroTransmisionGiro(long idGiro)
    {
      GIAdmisionGirosDC giro = GIRepositorioPagosManuales.Instancia.ConsultarGiroTransmisionGiro(idGiro);

      giro.AgenciaDestino = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentroServicio(giro.AgenciaDestino.IdCentroServicio);

      IADFachadaAdmisionesGiros fachadaGiros = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>();
      giro.RequiereDeclaracionVoluntaria = fachadaGiros.ValidarDeclaracionFondos(giro.Precio.ValorGiro);
      return giro;
    }

    /// <summary>
    ///  Insertar los intentos de transmision de un giro y
    ///  Actualizar la tabla giros indicando que el giro ya fue transmitido
    /// </summary>
    /// <param name="intestosTransmision"></param>
    public void InsertarIntentosTransmisionGiro(GIIntentosTransmisionGiroDC intentosTransmision)
    {
      using (TransactionScope transaccion = new TransactionScope())
      {
        ///Si el numero de la planilla es 0 o nulo, consulta la planilla del dia para la agencia
        if (intentosTransmision.IdPlanilla <= 0 || !intentosTransmision.IdPlanilla.HasValue)
        {
          intentosTransmision.IdPlanilla = GIRepositorioPagosManuales.Instancia.ObtenerNumeroPlanillaFechaActual(intentosTransmision.AgenciaDestino.IdCentroServicio);

          ///Si el numero de la planilla es 0 o nulo, obtiene el siguiente consecutivo para la planilla
          if (intentosTransmision.IdPlanilla == 0 || !intentosTransmision.IdPlanilla.HasValue)
            intentosTransmision.IdPlanilla = PAAdministrador.Instancia.ObtenerConsecutivo(PAEnumConsecutivos.Planilla_transmision_giros);
        }
        GIRepositorioPagosManuales.Instancia.InsertarIntentosTransmisionGiro(intentosTransmision);

        var pas = intentosTransmision.TipoPlanilla;

        if (intentosTransmision.TipoTransmision == GIEnumTipoTransmisionDC.TRA)
        {
          GIRepositorioPagosManuales.Instancia.ActualizarGiroTransmisionGiro(intentosTransmision.IdAdminGiro);
        }

        transaccion.Complete();
      }
    }

    /// <summary>
    /// Transmision de giros via fax
    /// </summary>
    /// <param name="intentosTransmision"></param>
    public long InsertarIntentosTransmisionFax(GIIntentosTransmisionGiroDC intentosTransmision)
    {
      intentosTransmision.TipoTransmision = GIEnumTipoTransmisionDC.TRA;
      intentosTransmision.TipoPlanilla = GIEnumTipoPlanillaTrasmisionDC.PLANILLA_FAX;

      using (TransactionScope transaccion = new TransactionScope())
      {
        intentosTransmision.IdPlanilla = PAAdministrador.Instancia.ObtenerConsecutivo(PAEnumConsecutivos.Planilla_transmision_giros);

        intentosTransmision.GirosTransmision.ForEach(giro =>
        {
          intentosTransmision.IdAdminGiro = giro.IdAdminGiro.Value;
          GIRepositorioPagosManuales.Instancia.InsertarIntentosTransmisionGiro(intentosTransmision);

          ///Actualiza la transmision de giros en la admision de giros
          GIRepositorioPagosManuales.Instancia.ActualizarGiroTransmisionGiro(intentosTransmision.IdAdminGiro);
        });

        transaccion.Complete();
      }
      return intentosTransmision.IdPlanilla.Value;
    }

    /// <summary>
    /// consulta los giros activos para realizar el descarge manual de pagos
    /// </summary>
    /// <param name="idCentroServicio"></param>
    /// <returns></returns>
    public IEnumerable<GIAdmisionGirosDC> ConsultarGirosADescargar(long? idCentroServicioDestino, long? idGiro, int indicePagina, int registrosPorPagina)
    {
      return GIRepositorioPagosManuales.Instancia.ConsultarGirosADescargar(idCentroServicioDestino, idGiro, indicePagina, registrosPorPagina);
    }

    /// <summary>
    /// consulta los giros activos para realizar el descarge manual de pagos
    /// por el id del giro y que correspondan al RACOL que esta logeado
    /// en el cliente
    /// </summary>
    /// <param name="lstCentroSrv">lista de centros de servicios de racol</param>
    /// <param name="idGiro">id del giro</param>
    /// <param name="indicePagina">indice de pagina</param>
    /// <param name="registrosPorPagina">registros por pagina</param>
    /// <returns>la info del giro encontrado</returns>
    public List<GIAdmisionGirosDC> ConsultarGiroADescargarPorRacol(ObservableCollection<PUCentroServiciosDC> lstCentroSrv, long? idGiro, int indicePagina, int registrosPorPagina)
    {
      List<GIAdmisionGirosDC> dataGiroReturn = new List<GIAdmisionGirosDC>();

      if (lstCentroSrv.Count > 0 && idGiro == null)
      {
        dataGiroReturn = new List<GIAdmisionGirosDC>(ConsultarGirosADescargar(lstCentroSrv.FirstOrDefault().IdCentroServicio, 0, indicePagina, registrosPorPagina));
      }
      else
      {
        List<GIAdmisionGirosDC> dataGiro = GIRepositorioPagosManuales.Instancia.ConsultarGirosADescargar(null, idGiro, indicePagina, registrosPorPagina);
        lstCentroSrv.ToList().ForEach(srv =>
        {
          if (dataGiro.FirstOrDefault().AgenciaDestino.IdCentroServicio == srv.IdCentroServicio)
          {
            dataGiroReturn.Add(dataGiro.FirstOrDefault());
          }
        });
      }
      return dataGiroReturn;
    }

    /// <summary>
    /// Realiza el pago del giro
    /// </summary>
    public void PagarGiro(PGPagosGirosDC pagosGiros)
    {
      using (TransactionScope scope = new TransactionScope())
      {
        if (!string.IsNullOrWhiteSpace(pagosGiros.ObservacionesComprobante))
        {
          pagosGiros.Observaciones = pagosGiros.ObservacionesComprobante + " " + pagosGiros.Observaciones;
        }
        else //validar que el comprobante de pago este para la agencia destino
        {
          ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
          PUCentroServiciosDC centroServicios = fachadaSuministros.ConsultarAgenciaPropietariaDelNumeroComprobante(pagosGiros.IdComprobantePago);
          if (centroServicios.IdCentroServicio != pagosGiros.IdCentroServiciosPagador)
          {
            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud.ERROR_COMPROBANTE_PAGO_AGENCIA.ToString(), GISolicitudesServidorMensajes.CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_COMPROBANTE_PAGO_AGENCIA)));
          }
        }

        pagosGiros = GIRepositorioPagosManuales.Instancia.ObtenerCorreos(pagosGiros);

        pagosGiros.PagoAutomatico = false;
        if (pagosGiros.EstadoGiro != GIConstantesSolicitudes.ESTADO_GIRO_DEV)
        {
          PGComprobantePagoDC comprobante = GIRepositorioPagosManuales.Instancia.PagarGiro(pagosGiros);
          //Integracion472.Instancia.IntegrarCuatroSieteDosPagoDevAnul(pagosGiros.IdAdmisionGiro,"1", DateTime.Now);
          // TODO: RON Prueba de deshabilitación integración

          /// Se Envia informacion a los modulos de comision y cajas
          IADFachadaAdmisionesGiros fachadaAdmisionesPago = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>();
          fachadaAdmisionesPago.EnviarTransaccionCaja(pagosGiros, comprobante);
        }
        else
        {
          bool retFlete = GIRepositorioPagosManuales.Instancia.ValidarMotivoDevRetornaFlete(pagosGiros.IdGiro.Value);

          if (retFlete)
          {
            //Se suma el valor del flete con el valor del giro, ya que el pago incluye los dos
            pagosGiros.ValorPagado = pagosGiros.ValorPagado + pagosGiros.ValorServicio;
          }

          PGComprobantePagoDC comprobante = GIRepositorioPagosManuales.Instancia.PagarGiro(pagosGiros);

          IADFachadaAdmisionesGiros fachadaAdmisionesPago = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>();
          if (retFlete)
          {
            //Se resta el valor del flete del valor del giro, para volver a los valores originales y así
            //poder afectar la caja con los dos conceptos de devolicion (retorno flete y retorno giro)
            pagosGiros.ValorPagado = pagosGiros.ValorPagado - pagosGiros.ValorServicio;

            /// Se Envia informacion a los modulos de comision y cajas para la devolucion del giro
            fachadaAdmisionesPago.EnviarTransaccionCaja(pagosGiros, comprobante, false, true);

            //se realiza la integración con 472 con estado anulado debido a que se retorna el valor del giro y la comision
            //En el documento de 472 dice que       2.	Giro Anulado: Es todo giro que por error de la empresa prestadora del servicio, error tecnológico o humano del (Front Office canal aliado) no pudo ser efectuado, por lo tanto se reversa toda la operación. (Monto del giro y comisión)
            //Integracion472.Instancia.IntegrarCuatroSieteDosPagoDevAnul(pagosGiros.IdAdmisionGiro, "2", DateTime.Now);
            // TODO: RON Prueba de deshabilitación integración
          }
          else
          {
            //se realizar la integracion con 472 con estado cancelado
            //segun el documento de 472    3.	Giro Cancelado: Estado giro que por razones del cliente/usuario – remitente, no se da continuidad, es decir que se reversa el giro mas no la comisión
            //Integracion472.Instancia.IntegrarCuatroSieteDosPagoDevAnul(pagosGiros.IdAdmisionGiro, "3", DateTime.Now);
            // TODO: RON Prueba de deshabilitación integración
          }
          /// Se Envia informacion a los modulos de comision y cajas para la devolucion del giro
          fachadaAdmisionesPago.EnviarTransaccionCaja(pagosGiros, comprobante, true, false);
        }

        scope.Complete();
      };

      ///Envia correo al remitente avisando pago del giro
      if (pagosGiros.EnviaCorreo && !string.IsNullOrEmpty(pagosGiros.CorreoRemitente))
        EnviarCorreo(pagosGiros);
    }

    /// <summary>
    /// Obtiene las planillas de trasmision para una agencia
    /// </summary>
    /// <returns></returns>
    public List<GIIntentosTransmisionGiroDC> ObtenerPlanillasTrasmisionAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idAgencia)
    {
      return GIRepositorioPagosManuales.Instancia.ObtenerPlanillasTrasmisionAgencia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idAgencia);
    }

    /// <summary>
    /// Obtiene la informacion de los intentos a transmitir de un giro
    /// </summary>
    /// <param name="idAdminGiro"></param>
    /// <returns></returns>
    public List<GIIntentosTransmisionGiroDC> ObtenerIntentosTransmitir(long idAdminGiro)
    {
      return GIRepositorioPagosManuales.Instancia.ObtenerIntentosTransmitir(idAdminGiro);
    }

    #region Notificacion correo

    /// <summary>
    /// Método para enviar un correo al remitente notificando el cobro del giro
    /// </summary>
    private void EnviarCorreo(PGPagosGirosDC pagoGiro)
    {
      InformacionAlerta informacionAlerta = PAParametros.Instancia.ConsultarInformacionAlerta(PAConstantesParametros.ALERTA_PAGO_GIRO);
      PAEnvioCorreoAsyn.Instancia.EnviarCorreoAsyn(pagoGiro.CorreoRemitente, informacionAlerta.Asunto, string.Format(informacionAlerta.Mensaje, pagoGiro.ClienteCobrador.Nombre + " " + pagoGiro.ClienteCobrador.Apellido1));
    }

    #endregion Notificacion correo
  }
}